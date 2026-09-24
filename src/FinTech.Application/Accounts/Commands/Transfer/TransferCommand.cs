using FinTech.Application.Common;
using MediatR;

namespace FinTech.Application.Accounts.Commands.Transfer;

public record TransferCommand(Guid OriginAccountId, Guid DestinationAccountId, decimal Amount, string Currency, string Reference, string IdempotencyKey)
    : IRequest<Guid>, IIdempotentRequest;
