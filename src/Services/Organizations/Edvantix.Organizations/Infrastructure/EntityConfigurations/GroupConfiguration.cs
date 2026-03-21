using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizations.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Organizations.Infrastructure.EntityConfigurations;

/// <summary>
/// EF Core configuration for <see cref="Group"/>.
/// HasQueryFilter is NOT set here — it is applied in <c>OrganizationsDbContext.OnModelCreating</c>
/// because it must combine both the tenant filter and the soft-delete filter in a single expression
/// (EF Core supports only one HasQueryFilter per entity).
/// </summary>
internal sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        // Sets HasKey(Id) and UUIDv7 default value
        builder.UseDefaultConfiguration();

        builder.ToTable("groups");

        builder.Property(g => g.IsDeleted).HasComment("Soft-delete flag");
        builder.Property(g => g.SchoolId).IsRequired();
        builder.Property(g => g.Name).IsRequired().HasMaxLength(150);
        builder.Property(g => g.Color).HasMaxLength(50);

        // Partial unique index: group name is unique per school among active (non-deleted) groups
        builder
            .HasIndex(g => new { g.SchoolId, g.Name })
            .IsUnique()
            .HasFilter("is_deleted = false");
    }
}
