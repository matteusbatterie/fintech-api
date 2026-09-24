using FinTech.Application.Common;
using MediatR;

namespace FinTech.Application.Accounts.Commands.Deposit;

public record DepositCommand(Guid AccountId, decimal Amount, string Currency, string IdempotencyKey)
    : IRequest, IIdempotentRequest;
