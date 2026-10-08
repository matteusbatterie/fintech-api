using FinTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTech.Infrastructure.Persistence.Configurations;

internal class EntryConfiguration : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.ToTable("Entries");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Description).HasMaxLength(250);
        builder.Property(e => e.Type);

        builder.OwnsOne(e => e.Amount, a =>
        {
            a.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)");

            a.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3);
        });
    }
}
