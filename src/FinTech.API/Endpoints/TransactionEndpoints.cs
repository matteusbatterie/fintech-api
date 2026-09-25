using FinTech.Application.Accounts.Commands.Transfer;
using MediatR;

namespace FinTech.API.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transactions").WithTags("Transactions");

        group.MapPost("/transfer", async (TransferRequest body, HttpRequest request, IMediator mediator, CancellationToken ct) =>
        {
            if (!request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey) || string.IsNullOrEmpty(idempotencyKey))
                return Results.BadRequest("Idempotency-Key header is required.");

            var command = new TransferCommand(
                body.OriginAccountId, body.DestinationAccountId, body.Amount, body.Currency, body.Reference, idempotencyKey!);
            var transactionId = await mediator.Send(command, ct);
            return Results.Created($"/api/transactions/{transactionId}", new { transactionId });
        });
    }
}

public record TransferRequest(Guid OriginAccountId, Guid DestinationAccountId, decimal Amount, string Currency, string Reference);
