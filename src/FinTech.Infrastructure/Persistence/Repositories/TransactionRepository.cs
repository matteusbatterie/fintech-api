using FinTech.Domain.Entities;
using FinTech.Domain.Interfaces;

namespace FinTech.Infrastructure.Persistence.Repositories;

public class TransactionRepository(FinTechDbContext context) : ITransactionRepository
{
    public async Task AddAsync(Transaction transaction)
        => await context.Transactions.AddAsync(transaction);
}
