using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Участник организации с определённой ролью.
/// </summary>
public sealed class OrganizationMember() : Entity<Guid>, IAggregateRoot, ISoftDelete
{
    public OrganizationMember(long organizationId, long profileId, OrganizationRole role)
        : this()
    {
        if (organizationId <= 0)
            throw new ArgumentException(
                "Некорректный идентификатор организации.",
                nameof(organizationId)
            );

        if (profileId <= 0)
            throw new ArgumentException("Некорректный идентификатор профиля.", nameof(profileId));

        OrganizationId = organizationId;
        ProfileId = profileId;
        Role = role;
        JoinedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public long OrganizationId { get; private set; }
    public OrganizationAggregate.Organization Organization { get; private set; } = null!;
    public long ProfileId { get; private set; }
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
