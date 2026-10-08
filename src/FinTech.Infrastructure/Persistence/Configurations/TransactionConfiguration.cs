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
        builder.HasMany(t => t.Entries)
            .WithOne()
            .HasForeignKey("TransactionId")     // Shadow FK
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);  // Deleting a Transaction will delete its Entries
    }
}
