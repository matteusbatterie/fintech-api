using FinTech.Domain.Entities;
using FinTech.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinTech.Infrastructure.Persistence.Repositories;

public class AccountRepository(FinTechDbContext context) : IAccountRepository
{
    public async Task<Account?> GetByIdAsync(Guid id)
        => await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);

    public async Task AddAsync(Account account)
        => await context.Accounts.AddAsync(account);

    public void Update(Account account)
    {
        if (context.Entry(account).State == EntityState.Detached)
            context.Accounts.Attach(account).State = EntityState.Modified;
        // If it's already tracked, this is a no-op; EF already sees the changes
        // made by Deposit()/Withdraw(). This just guards the detached case.
    }
}
