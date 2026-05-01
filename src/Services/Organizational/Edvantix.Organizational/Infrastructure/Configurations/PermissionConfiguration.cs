using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(p => p.Code).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

        // Feature всегда загружается вместе с разрешением — нужен для маппинга в DTO.
        builder.Navigation(p => p.Feature).AutoInclude();

        // Код разрешения уникален в рамках одной функциональной области.
        builder.HasIndex(p => new { p.FeatureId, p.Code }).IsUnique();
    }
}
