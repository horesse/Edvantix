namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Участник организации с определённой ролью.
/// </summary>
public sealed class OrganizationMember() : Entity<Guid>, IAggregateRoot, ISoftDelete
{
    public OrganizationMember(Guid organizationId, Guid profileId, OrganizationRole role)
        : this()
    {
        OrganizationId = organizationId;
        ProfileId = profileId;
        Role = role;
        JoinedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public Guid ProfileId { get; private set; }
    public OrganizationRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Изменяет роль участника.
    /// </summary>
    public void UpdateRole(OrganizationRole newRole)
    {
        Role = newRole;
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Участник организации уже удалён.");

        IsDeleted = true;
    }
}
