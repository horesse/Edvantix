using Edvantix.Chassis.EF.Configurations;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Groups.Infrastructure.Configurations;

internal sealed class LevelConfiguration : IEntityTypeConfiguration<Level>
{
    public void Configure(EntityTypeBuilder<Level> builder)
    {
        builder.ConfigureSoftDeletable();

        // LevelCode хранится как строка через конвертер value object → string
        builder
            .Property(l => l.Code)
            .IsRequired()
            .HasMaxLength(16)
            .HasConversion(c => c.Value, s => LevelCode.From(s));

        builder.Property(l => l.Name).IsRequired().HasMaxLength(64);
        builder.Property(l => l.Description).IsRequired(false).HasMaxLength(256);

        builder
            .Property(l => l.Tone)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder.Property(l => l.SortOrder).IsRequired();
        builder.Property(l => l.IsActive).IsRequired();

        // Уникальный код в рамках организации (только среди не удалённых)
        builder
            .HasIndex(l => new { l.OrganizationId, l.Code })
            .IsUnique()
            .HasFilter("is_deleted = false");

        // Уникальный порядковый номер в рамках организации (только среди не удалённых)
        builder
            .HasIndex(l => new { l.OrganizationId, l.SortOrder })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.HasIndex(l => new { l.OrganizationId, l.IsActive });
    }
}
