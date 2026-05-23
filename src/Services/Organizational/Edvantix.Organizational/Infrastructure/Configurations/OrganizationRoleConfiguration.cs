using Edvantix.Chassis.EF.Configurations;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class OrganizationRoleConfiguration : IEntityTypeConfiguration<OrganizationRole>
{
    public void Configure(EntityTypeBuilder<OrganizationRole> builder)
    {
        builder.ConfigureSoftDeletable();

        builder.Property(r => r.Name).IsRequired().HasMaxLength(DataSchemaLength.Medium);
        builder.Property(r => r.Description).HasMaxLength(DataSchemaLength.Large);

        builder.HasIndex(r => new { r.OrganizationId, r.Name }).IsUnique();

        builder
            .HasMany(r => r.Permissions)
            .WithMany(p => p.OrganizationRoles)
            .UsingEntity<OrganizationRolePermission>();
    }
}
