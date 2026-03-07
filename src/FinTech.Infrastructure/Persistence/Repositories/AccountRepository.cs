using FinTech.Domain.Entities;
using FinTech.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinTech.Infrastructure.Persistence.Repositories;

public class AccountRepository(FinTechDbContext context) : IAccountRepository
{
    public async Task<Account?> GetByIdAsync(Guid id)
    {
        return await context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task SaveAsync(Account account, Transaction transaction)
    {
        var accountEntry = context.Entry(account);
        if (accountEntry.State == EntityState.Detached)
        {
            context.Accounts.Update(account);
        }

        await context.Transactions.AddAsync(transaction);

        await context.SaveChangesAsync();
    }
}
