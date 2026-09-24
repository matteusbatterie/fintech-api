using FinTech.Domain.Interfaces;
using MediatR;

namespace FinTech.Application.Accounts.Queries.GetAccountById;

public class GetAccountByIdQueryHandler(IAccountRepository accountRepository)
    : IRequestHandler<GetAccountByIdQuery, AccountDto?>
{
    public async Task<AccountDto?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId);

        if (account is null) return null;

        return new AccountDto(account.Id, account.Name, account.Balance.Amount, account.Balance.Currency, account.CreatedAt);
    }
}
