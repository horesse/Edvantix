using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class OrganizationMemberRoleConfiguration
    : IEntityTypeConfiguration<OrganizationMemberRole>
{
    public void Configure(EntityTypeBuilder<OrganizationMemberRole> builder)
    {
        builder.ConfigureSoftDeletable();

        builder.Property(r => r.Code).IsRequired().HasMaxLength(DataSchemaLength.Medium);
        builder.Property(r => r.Description).HasMaxLength(DataSchemaLength.Large);

        builder.HasIndex(r => new { r.OrganizationId, r.Code }).IsUnique();

        builder
            .HasMany(r => r.Permissions)
            .WithMany(p => p.OrganizationMemberRoles)
            .UsingEntity<OrganizationMemberRolePermission>();
    }
}
