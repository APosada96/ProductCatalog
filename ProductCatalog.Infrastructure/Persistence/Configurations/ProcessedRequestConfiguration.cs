using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Infrastructure.Idempotency;

namespace ProductCatalog.Infrastructure.Persistence.Configurations;

public sealed class ProcessedRequestConfiguration : IEntityTypeConfiguration<ProcessedRequest>
{
    public void Configure(EntityTypeBuilder<ProcessedRequest> b)
    {
        b.ToTable("ProcessedRequests");

        b.HasKey(x => x.Id);

        b.Property(x => x.Key)
            .HasMaxLength(200)
            .IsRequired();

        b.HasIndex(x => x.Key)
            .IsUnique()
            .HasDatabaseName("UX_ProcessedRequests_Key");

        b.Property(x => x.ProcessedAt)
            .IsRequired();
    }
}
