using FinTech.Domain.Interfaces;
using MediatR;

namespace FinTech.Application.Accounts.Commands.Deposit;

public record DepositCommand(Guid AccountId, decimal Amount, string Currency)
    : IRequest;
