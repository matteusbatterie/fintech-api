using FinTech.Domain.Entities;

namespace FinTech.Domain.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id);
    Task AddAsync(Account account);
    void Update(Account account);
}
