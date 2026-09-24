using FinTech.Infrastructure.Idempotency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTech.Infrastructure.Persistence.Configurations;

public class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable("IdempotencyRecords");
        builder.HasKey(r => r.Key);

        builder.Property(r => r.Key).IsRequired();
        builder.Property(r => r.RequestType).HasMaxLength(200).IsRequired();
        builder.Property(r => r.RequestHash).HasMaxLength(64).IsRequired();

        // Reduntant, but documents intent when reading the schema.
        builder.HasIndex(r => r.Key).IsUnique();
    }
}
