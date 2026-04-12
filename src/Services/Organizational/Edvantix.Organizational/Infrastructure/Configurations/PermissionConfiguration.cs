using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(p => p.Feature).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

        // Unique per feature — the same permission name can exist in different features.
        builder.HasIndex(p => new { p.Feature, p.Name }).IsUnique();
    }
}
