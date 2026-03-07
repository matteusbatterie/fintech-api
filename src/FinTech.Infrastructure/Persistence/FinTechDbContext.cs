using FinTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinTech.Infrastructure.Persistence;

public class FinTechDbContext(DbContextOptions<FinTechDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinTechDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
