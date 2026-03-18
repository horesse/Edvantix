using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizations.Domain.AggregatesModel.RoleAggregate;

namespace Edvantix.Organizations.Infrastructure.EntityConfigurations;

/// <summary>
/// EF Core configuration for <see cref="Role"/>.
/// HasQueryFilter is NOT set here — it is applied in <c>OrganizationsDbContext.OnModelCreating</c>
/// because it must combine both the tenant filter and the soft-delete filter in a single expression
/// (EF Core supports only one HasQueryFilter per entity).
/// </summary>
internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Sets HasKey(Id) and UUIDv7 default value
        builder.UseDefaultConfiguration();

        builder.Property(r => r.IsDeleted).HasComment("Soft-delete flag");
        builder.Property(r => r.SchoolId).IsRequired();
        builder.Property(r => r.Name).IsRequired().HasMaxLength(150);

        // Partial unique index: role name is unique per school among active (non-deleted) roles
        builder
            .HasIndex(r => new { r.SchoolId, r.Name })
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder
            .HasMany(r => r.Permissions)
            .WithOne()
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Use backing field for Permissions collection so EF Core tracks changes made
        // via AssignPermission/RemovePermission/SetPermissions without exposing a setter.
        builder
            .Metadata.FindNavigation(nameof(Role.Permissions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
