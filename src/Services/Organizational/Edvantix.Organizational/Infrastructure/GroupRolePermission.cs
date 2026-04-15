namespace Edvantix.Organizational.Infrastructure;

/// <summary>Промежуточная таблица связи GroupRole ↔ Permission.</summary>
internal sealed class GroupRolePermission
{
    public Guid GroupRoleId { get; set; }
    public Guid PermissionId { get; set; }
}
