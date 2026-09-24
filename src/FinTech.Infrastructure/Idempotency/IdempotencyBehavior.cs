using System.ComponentModel;
using System.Security.Cryptography;
using System.Text.Json;
using FinTech.Application.Common;
using FinTech.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FinTech.Infrastructure.Idempotency;

public class IdempotencyBehavior<TRequest, TResponse>(FinTechDbContext context)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IIdempotentRequest
{
    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var requestHash = ComputeHash(request);

        var existing = await context.IdempotencyRecords
            .FirstOrDefaultAsync(r => r.Key == request.IdempotencyKey, ct);

        if (existing is not null)
        {
            if (existing.RequestHash != requestHash)
                throw new InvalidOperationException(
                    "This idempotency request was already used with a different request payload.");

            if (existing.IsCompleted)
                return JsonSerializer.Deserialize<TResponse>(existing.ResponsePayload!)!;

            throw new InvalidOperationException(
                "A request with this idempotency key is already being processed.");
        }

        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        var record = new IdempotencyRecord
        {
            Key = request.IdempotencyKey,
            RequestType = typeof(TRequest).Name,
            RequestHash = requestHash,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        context.IdempotencyRecords.Add(record);

        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch(DbUpdateException ex)
        {
            await transaction.RollbackAsync(ct);

            // Assuming the concurrent duplicate key is the most likely cause of the exception,
            // we can throw a more specific exception here.
            throw new InvalidOperationException(
                "A request with this idempotency key is already being processed.", ex);
        }

        TResponse response;

        try
        {
            response = await next(ct);  // handler runs, calls its own IUnitOfWork.SaveChangesAsync —
                                        // same ambient transaction, so it's part of this commit
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;  // domain/validation failure; nothing persists, key isn't burned
        }


        record.ResponsePayload = JsonSerializer.Serialize(response);
        record.IsCompleted = true;
        await context.SaveChangesAsync(ct);

        await transaction.CommitAsync(ct);
        return response;
    }

    private static string ComputeHash(TRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var hashBytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hashBytes);
    }
}
