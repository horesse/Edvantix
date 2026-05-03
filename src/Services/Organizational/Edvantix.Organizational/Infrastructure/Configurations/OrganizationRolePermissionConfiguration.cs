using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class OrganizationRolePermissionConfiguration
    : IEntityTypeConfiguration<OrganizationRolePermission>
{
    public void Configure(EntityTypeBuilder<OrganizationRolePermission> builder)
    {
        builder.HasKey(x => new
        {
            OrganizationMemberRoleId = x.OrganizationRoleId,
            x.PermissionId,
        });

        builder.Property(x => x.OrganizationRoleId);
    }
}
