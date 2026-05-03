namespace Edvantix.Organizational.Features.Roles;

/// <summary>DTO элемента списка ролей.</summary>
public sealed record RoleDto(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string? Description,
    bool IsSystem,
    bool IsOwner,
    int PermissionsCount
)
{
    /// <summary>Общее количество доступных разрешений в системе.</summary>
    public int TotalPermissionsCount { get; init; }

    /// <summary>Количество участников с этой ролью.</summary>
    public int MembersCount { get; init; }
}

/// <summary>DTO детального просмотра роли (GetById).</summary>
public sealed record RoleDetailDto(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string? Description,
    bool IsSystem,
    bool IsOwner
)
{
    /// <summary>Все разрешения системы, сгруппированные по функциональной области, с флагом активации.</summary>
    public IReadOnlyList<FeatureDto> Features { get; init; } = [];

    /// <summary>Общее количество доступных разрешений в системе.</summary>
    public int TotalPermissionsCount { get; init; }

    /// <summary>Количество участников с этой ролью.</summary>
    public int MembersCount { get; init; }
}

/// <summary>Функциональная область с набором разрешений и флагами активации.</summary>
public sealed record FeatureDto(string Code, string Name, IReadOnlyList<PermissionDto> Permissions);

/// <summary>Разрешение с флагом активации в контексте роли.</summary>
public sealed record PermissionDto(Guid Id, string Code, string Name, bool IsActive);
