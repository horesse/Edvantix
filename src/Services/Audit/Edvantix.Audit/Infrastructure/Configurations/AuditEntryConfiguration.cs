using Edvantix.Audit.Domain.AggregatesModel.AuditEntryAggregate;
using Edvantix.SharedKernel.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Audit.Infrastructure.Configurations;

internal sealed class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(p => p.Id).HasDefaultValueSql(UniqueIdentifierHelper.NewUuidV7);

        builder.Property(p => p.OccurredAt).HasDefaultValueSql(DateTimeHelper.SqlUtcNow);

        builder
            .Property(e => e.Action)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Medium)
            .HasConversion<string>();

        builder
            .Property(e => e.EntityType)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Medium)
            .HasConversion<string>();

        builder.Property(e => e.Description).HasMaxLength(DataSchemaLength.Large);

        builder.Property(e => e.IpAddress).HasMaxLength(DataSchemaLength.Small);

        builder.Property(e => e.UserAgent).HasMaxLength(DataSchemaLength.SuperLarge);

        // Индексы для типичных запросов
        builder.HasIndex(e => new { e.OrganizationId, e.OccurredAt });
        builder.HasIndex(e => new { e.OrganizationId, e.ActorId });
        builder.HasIndex(e => new
        {
            e.OrganizationId,
            e.EntityType,
            e.EntityId,
        });
    }
}
