using FinTech.Domain.Interfaces;

namespace FinTech.Infrastructure.Persistence;

public class UnitOfWork(FinTechDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => context.SaveChangesAsync(cancellationToken);
}
