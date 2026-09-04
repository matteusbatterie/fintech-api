using FinTech.Domain.Entities;

namespace FinTech.Domain.Interfaces;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);
}
