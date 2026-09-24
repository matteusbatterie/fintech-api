using MediatR;

namespace FinTech.Application.Accounts.Commands.Withdraw;

public record WithdrawCommand(Guid AccountId, decimal Amount, string Currency)
    : IRequest;
