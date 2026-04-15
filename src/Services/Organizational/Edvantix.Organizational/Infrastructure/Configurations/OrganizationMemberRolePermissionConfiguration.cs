using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class OrganizationMemberRolePermissionConfiguration
    : IEntityTypeConfiguration<OrganizationMemberRolePermission>
{
    public void Configure(EntityTypeBuilder<OrganizationMemberRolePermission> builder)
    {
        builder.HasKey(x => new { x.OrganizationMemberRoleId, x.PermissionId });

        builder.Property(x => x.OrganizationMemberRoleId);
    }
}
