using FinTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTech.Infrastructure.Persistence.Configurations;

internal class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Value Object: Balance (Money)
        builder.OwnsOne(a => a.Balance, b =>
        {
            b.Property(m => m.Amount)
                .HasColumnName("BalanceAmount")
                .HasColumnType("decimal(18,2)");

            b.Property(m => m.Currency)
                .HasColumnName("BalanceCurrency")
                .HasMaxLength(3);
        });

        // Value Object: Document
        builder.OwnsOne(a => a.Document, d =>
        {
            d.Property(doc => doc.Number)
                .HasColumnName("DocumentNumber")
                .HasMaxLength(20);

            d.Property(doc => doc.Type)
                .HasColumnName("DocumentType")
                .HasMaxLength(10);
        });
    }
}
