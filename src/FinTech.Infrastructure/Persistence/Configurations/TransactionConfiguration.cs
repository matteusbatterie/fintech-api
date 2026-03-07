using FinTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTech.Infrastructure.Persistence.Configurations;

internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Reference).HasMaxLength(250);

        // Map the collection of Entries
        builder.OwnsMany(t => t.Entries, e =>
        {
            e.ToTable("Entries");
            e.WithOwner().HasForeignKey("TransactionId");

            e.Property<Guid>("Id");
            e.HasKey("Id");

            e.OwnsOne(entry => entry.Amount, a => {
                a.Property(m => m.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)");
                a.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
            });
        });
    }
}
