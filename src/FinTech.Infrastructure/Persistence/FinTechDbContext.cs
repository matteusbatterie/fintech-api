using FinTech.Domain.Common;
using FinTech.Domain.Entities;
using FinTech.Infrastructure.Idempotency;
using Microsoft.EntityFrameworkCore;

namespace FinTech.Infrastructure.Persistence;

public class FinTechDbContext(DbContextOptions<FinTechDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<DomainEvent>();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinTechDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
