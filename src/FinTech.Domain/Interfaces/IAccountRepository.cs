using FinTech.Domain.Entities;

namespace FinTech.Domain.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(Guid id);
    void Update(Account account);
}
