using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizations.Domain.AggregatesModel.UserRoleAssignmentAggregate;

namespace Edvantix.Organizations.Infrastructure.EntityConfigurations;

/// <summary>
/// EF Core configuration for <see cref="UserRoleAssignment"/>.
/// HasQueryFilter is NOT set here — it is applied in <c>OrganizationsDbContext.OnModelCreating</c>
/// to allow ITenantContext injection.
/// </summary>
internal sealed class UserRoleAssignmentConfiguration : IEntityTypeConfiguration<UserRoleAssignment>
{
    public void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(a => a.ProfileId).IsRequired();
        builder.Property(a => a.SchoolId).IsRequired();
        builder.Property(a => a.RoleId).IsRequired();

        // A user cannot hold the same role twice in the same school
        builder
            .HasIndex(a => new
            {
                a.ProfileId,
                a.SchoolId,
                a.RoleId,
            })
            .IsUnique();
    }
}
