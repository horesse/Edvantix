using Edvantix.Organizational.Domain.Enums;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Текущий факт участия пользователя в учебной группе.
/// Один пользователь может одновременно быть членом нескольких групп с разными ролями.
/// </summary>
public sealed class GroupMember() : Entity, ITenanted
{
    /// <param name="organizationId">Идентификатор организации (для мультиарендности).</param>
    /// <param name="groupId">Идентификатор группы.</param>
    /// <param name="profileId">Идентификатор пользователя из Profile Service.</param>
    /// <param name="groupRoleId">Идентификатор роли участника в группе.</param>
    /// <param name="joinedAt">Дата вступления в группу.</param>
    public GroupMember(
        Guid organizationId,
        Guid groupId,
        Guid profileId,
        Guid groupRoleId,
        DateOnly joinedAt
    )
        : this()
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );
        if (groupId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор группы не может быть пустым.",
                nameof(groupId)
            );
        if (profileId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор профиля не может быть пустым.",
                nameof(profileId)
            );
        if (groupRoleId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор роли не может быть пустым.",
                nameof(groupRoleId)
            );

        OrganizationId = organizationId;
        GroupId = groupId;
        ProfileId = profileId;
        GroupRoleId = groupRoleId;
        Status = OrganizationStatus.Active;
        JoinedAt = joinedAt;
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>Идентификатор группы.</summary>
    public Guid GroupId { get; private set; }

    /// <summary>Идентификатор пользователя из Profile Service.</summary>
    public Guid ProfileId { get; private set; }

    /// <summary>Идентификатор роли участника в данной группе.</summary>
    public Guid GroupRoleId { get; private set; }

    /// <summary>Текущий статус участника группы.</summary>
    public OrganizationStatus Status { get; private set; }

    /// <summary>Дата вступления участника в группу.</summary>
    public DateOnly JoinedAt { get; private set; }

    /// <summary>
    /// Дата выхода участника из группы.
    /// null означает, что участник всё ещё состоит в группе.
    /// </summary>
    public DateOnly? ExitedAt { get; private set; }

    /// <summary>Причина выхода из группы (отчисление, перевод, завершение курса и т.д.).</summary>
    public string? ExitReason { get; private set; }

    /// <summary>Изменяет роль участника в группе.</summary>
    public void ChangeRole(Guid groupRoleId)
    {
        if (groupRoleId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор роли не может быть пустым.",
                nameof(groupRoleId)
            );

        GroupRoleId = groupRoleId;
    }

    /// <summary>Фиксирует выход участника из группы и обновляет запись в истории.</summary>
    /// <param name="exitedAt">Дата выхода.</param>
    /// <param name="exitReason">Причина выхода.</param>
    public void Exit(DateOnly exitedAt, string? exitReason = null)
    {
        ExitedAt = exitedAt;
        ExitReason = exitReason;

        Status = OrganizationStatus.Archived;
    }
}
