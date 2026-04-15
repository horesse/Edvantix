using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class GroupRoleConfiguration : IEntityTypeConfiguration<GroupRole>
{
    public void Configure(EntityTypeBuilder<GroupRole> builder)
    {
        builder.ConfigureSoftDeletable();

        builder.Property(r => r.Code).IsRequired().HasMaxLength(DataSchemaLength.Medium);
        builder.Property(r => r.Description).HasMaxLength(DataSchemaLength.Large);

        builder.HasIndex(r => new { r.OrganizationId, r.Code }).IsUnique();

        builder
            .HasMany(r => r.Permissions)
            .WithMany(p => p.GroupRoles)
            .UsingEntity<GroupRolePermission>();
    }
}
