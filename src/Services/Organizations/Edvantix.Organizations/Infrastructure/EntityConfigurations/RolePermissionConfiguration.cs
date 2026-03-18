using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizations.Domain.AggregatesModel.RoleAggregate;

namespace Edvantix.Organizations.Infrastructure.EntityConfigurations;

/// <summary>
/// EF Core configuration for <see cref="RolePermission"/>.
/// This is the join entity between <see cref="Role"/> and the global permission catalogue.
/// </summary>
internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(rp => rp.RoleId).IsRequired();
        builder.Property(rp => rp.PermissionId).IsRequired();

        // A permission can only be assigned to a role once
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId }).IsUnique();
    }
}
