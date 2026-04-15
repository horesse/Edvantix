using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class GroupRolePermissionConfiguration
    : IEntityTypeConfiguration<GroupRolePermission>
{
    public void Configure(EntityTypeBuilder<GroupRolePermission> builder)
    {
        builder.HasKey(x => new { x.GroupRoleId, x.PermissionId });

        builder.Property(x => x.GroupRoleId);
    }
}
