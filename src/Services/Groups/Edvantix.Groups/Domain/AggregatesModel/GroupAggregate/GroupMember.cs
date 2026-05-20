using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Текущий факт участия пользователя в учебной группе.
/// Один пользователь может одновременно состоять в нескольких группах с разными ролями.
/// </summary>
public sealed class GroupMember() : Entity, ITenanted
{
    /// <param name="organizationId">Идентификатор организации (для мультиарендности).</param>
    /// <param name="groupId">Идентификатор группы.</param>
    /// <param name="profileId">Идентификатор пользователя из Profile Service.</param>
    /// <param name="role">Роль участника внутри группы.</param>
    /// <param name="joinedAt">Дата вступления в группу.</param>
    public GroupMember(
        Guid organizationId,
        Guid groupId,
        Guid profileId,
        GroupMemberRole role,
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

        OrganizationId = organizationId;
        GroupId = groupId;
        ProfileId = profileId;
        Role = role;
        JoinedAt = joinedAt;
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>Идентификатор группы.</summary>
    public Guid GroupId { get; private set; }

    /// <summary>Идентификатор пользователя из Profile Service.</summary>
    public Guid ProfileId { get; private set; }

    /// <summary>Роль участника внутри группы.</summary>
    public GroupMemberRole Role { get; private set; }

    /// <summary>Дата вступления участника в группу.</summary>
    public DateOnly JoinedAt { get; private set; }

    /// <summary>
    /// Дата выхода участника из группы.
    /// <c>null</c> означает, что участник всё ещё состоит в группе.
    /// </summary>
    public DateOnly? ExitedAt { get; private set; }

    /// <summary>Причина выхода из группы (отчисление, перевод, завершение курса и т.д.).</summary>
    public string? ExitReason { get; private set; }

    /// <summary>Фиксирует выход участника из группы и обновляет запись в истории.</summary>
    public void Exit(DateOnly exitedAt, string? exitReason = null)
    {
        ExitedAt = exitedAt;
        ExitReason = exitReason;
    }
}
