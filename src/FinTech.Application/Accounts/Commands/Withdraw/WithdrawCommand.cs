using FinTech.Application.Common;
using MediatR;

namespace FinTech.Application.Accounts.Commands.Withdraw;

public record WithdrawCommand(Guid AccountId, decimal Amount, string Currency, string IdempotencyKey)
    : IRequest, IIdempotentRequest;
