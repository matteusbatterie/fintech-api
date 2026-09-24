using MediatR;

namespace FinTech.Application.Accounts.Commands.Transfer;

public record TransferCommand(Guid OriginAccountId, Guid DestinationAccountId, decimal Amount, string Currency, string Reference)
    : IRequest<Guid>;
