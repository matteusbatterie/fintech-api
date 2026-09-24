using MediatR;

namespace FinTech.Application.Accounts.Queries.GetAccountById;

public record GetAccountByIdQuery(Guid AccountId) : IRequest<AccountDto>;
