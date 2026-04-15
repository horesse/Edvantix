using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Organizational.Infrastructure.Configurations;

internal sealed class GroupRolePermissionConfiguration
    : IEntityTypeConfiguration<GroupRolePermission>
{
    public void Configure(EntityTypeBuilder<GroupRolePermission> builder)
    {
        builder.ToTable("group_role_permissions");
        builder.HasKey(x => new { x.GroupRoleId, x.PermissionId });

        // NamingConventions даёт group_role_id — переименовываем в role_id.
        builder.Property(x => x.GroupRoleId).HasColumnName("role_id");

        // PermissionId → permission_id совпадает с конвенцией, явного HasColumnName не нужно.
    }
}
