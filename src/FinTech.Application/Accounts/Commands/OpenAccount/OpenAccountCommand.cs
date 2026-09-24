using MediatR;

namespace FinTech.Application.Accounts.Commands.OpenAccount;

public record OpenAccountCommand(string Name, string DocumentNumber, string DocumentType, string Currency)
    : IRequest<Guid>;
