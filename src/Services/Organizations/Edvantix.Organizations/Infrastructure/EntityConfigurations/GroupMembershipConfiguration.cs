using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizations.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Organizations.Infrastructure.EntityConfigurations;

/// <summary>
/// EF Core configuration for <see cref="GroupMembership"/>.
/// HasQueryFilter is applied in <c>OrganizationsDbContext.OnModelCreating</c>
/// rather than here, because it must reference the injected <see cref="ITenantContext"/>.
/// </summary>
internal sealed class GroupMembershipConfiguration : IEntityTypeConfiguration<GroupMembership>
{
    public void Configure(EntityTypeBuilder<GroupMembership> builder)
    {
        builder.UseDefaultConfiguration();

        builder.ToTable("group_memberships");

        builder.Property(m => m.GroupId).IsRequired();
        builder.Property(m => m.ProfileId).IsRequired();
        builder.Property(m => m.SchoolId).IsRequired();

        // Store as timestamp with time zone (UTC) — consistent with PostgreSQL best practices.
        builder.Property(m => m.AddedAt).HasColumnType("timestamp with time zone").IsRequired();

        // A student can only belong to a group once per school.
        builder.HasIndex(m => new { m.GroupId, m.ProfileId }).IsUnique();

        // Relationship back to the Group aggregate root.
        // EF Core auto-discovers the _members backing field from the Members property.
        // Using the property name "Members" avoids duplicate field mapping errors
        // (EF Core would conflict if we specify "_members" and auto-discover it simultaneously).
        builder
            .HasOne<Group>()
            .WithMany(nameof(Group.Members))
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
