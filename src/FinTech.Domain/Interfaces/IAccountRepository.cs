using FinTech.Domain.Entities;

namespace FinTech.Domain.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id);

    Task SaveAsync(Account account, Transaction transaction);
}
