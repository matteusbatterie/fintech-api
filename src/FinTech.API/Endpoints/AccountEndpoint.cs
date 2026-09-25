using System.Security.Cryptography;
using Azure.Core;
using FinTech.Application.Accounts.Commands.Deposit;
using FinTech.Application.Accounts.Commands.OpenAccount;
using FinTech.Application.Accounts.Commands.Withdraw;
using FinTech.Application.Accounts.Queries.GetAccountById;
using MediatR;

namespace FinTech.API.Endpoints;

public static class AccountEndpoint
{
    public static void MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/accounts").WithTags("Accounts");

        group.MapPost("/", async (OpenAccountCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var id = await mediator.Send(command, ct);
            return Results.Created($"/api/accounts/{id}", new { id });
        });

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var account = await mediator.Send(new GetAccountByIdQuery(id), ct);
            return account is not null ? Results.Ok(account) : Results.NotFound();
        });

        group.MapPost("/{id:guid}/deposit", async (Guid id, DepositRequest body, HttpRequest request,IMediator mediator, CancellationToken ct) =>
        {
            if (!request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey) || string.IsNullOrEmpty(idempotencyKey))
                return Results.BadRequest("Idempotency-Key header is required.");

            var command = new DepositCommand(id, body.Amount, body.Currency, idempotencyKey!);
            await mediator.Send(command, ct);
            return Results.NoContent();
        });

        group.MapPost("/{id:guid}/withdraw", async (Guid id, WithdrawRequest body, HttpRequest request, IMediator mediator, CancellationToken ct) =>
        {
            if (!request.Headers.TryGetValue("Idempotency-Key", out var idempotencyKey) || string.IsNullOrEmpty(idempotencyKey))
                return Results.BadRequest("Idempotency-Key header is required.");

            var command = new WithdrawCommand(id, body.Amount, body.Currency, idempotencyKey!);
            await mediator.Send(command, ct);
            return Results.NoContent();
        });
    }
}

public record DepositRequest(decimal Amount, string Currency);
public record WithdrawRequest(decimal Amount, string Currency);
