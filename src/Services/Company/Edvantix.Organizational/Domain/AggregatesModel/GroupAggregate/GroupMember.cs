namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Участник группы с определённой ролью в группе.
/// </summary>
public sealed class GroupMember() : Entity<Guid>, IAggregateRoot, ISoftDelete
{
    public GroupMember(ulong groupId, ulong profileId, GroupRole role)
        : this()
    {
        if (groupId <= 0)
            throw new ArgumentException("Некорректный идентификатор группы.", nameof(groupId));

        if (profileId <= 0)
            throw new ArgumentException("Некорректный идентификатор профиля.", nameof(profileId));

        GroupId = groupId;
        ProfileId = profileId;
        Role = role;
        JoinedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public ulong GroupId { get; private set; }
    public Group Group { get; private set; } = null!;
    public ulong ProfileId { get; private set; }
    public GroupRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Изменяет роль участника группы.
    /// </summary>
    public void UpdateRole(GroupRole newRole)
    {
        Role = newRole;
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Участник группы уже удалён.");

        IsDeleted = true;
    }
}
