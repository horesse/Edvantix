using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizations.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizations.Infrastructure.EntityConfigurations;

/// <summary>
/// EF Core configuration for <see cref="Permission"/>.
/// No HasQueryFilter — Permission is a global catalogue shared across all tenants.
/// </summary>
internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

        // Permission names must be globally unique (e.g., "scheduling:read")
        builder.HasIndex(p => p.Name).IsUnique();
    }
}
