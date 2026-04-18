using Edvantix.Organizational.Domain.Enums;
using Edvantix.Organizational.Domain.Events;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Участник организации — факт принадлежности пользователя к организации и его роль в ней.
/// Один пользователь (<see cref="ProfileId"/>) может быть участником нескольких организаций
/// с разными ролями.
/// </summary>
public sealed class OrganizationMember() : Entity, IAggregateRoot, ISoftDelete, ITenanted
{
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="profileId">Идентификатор пользователя из Profile Service.</param>
    /// <param name="organizationMemberRoleId">Идентификатор кастомной роли участника.</param>
    /// <param name="startDate">Дата начала участия.</param>
    /// <param name="endDate">Дата окончания участия; null означает, что участие актуально.</param>
    public OrganizationMember(
        Guid organizationId,
        Guid profileId,
        Guid organizationMemberRoleId,
        DateOnly startDate,
        DateOnly? endDate = null
    )
        : this()
    {
        Id = Guid.CreateVersion7();
        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );
        if (profileId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор профиля не может быть пустым.",
                nameof(profileId)
            );
        if (organizationMemberRoleId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор роли не может быть пустым.",
                nameof(organizationMemberRoleId)
            );

        if (endDate.HasValue && endDate.Value < startDate)
        {
            throw new ArgumentException(
                "Дата окончания участия не может быть раньше даты начала.",
                nameof(endDate)
            );
        }

        OrganizationId = organizationId;
        ProfileId = profileId;
        OrganizationMemberRoleId = organizationMemberRoleId;
        StartDate = startDate;
        EndDate = endDate;
        Status = OrganizationStatus.Active;
        IsDeleted = false;

        RegisterDomainEvent(new OrganizationMemberCreatedDomainEvent(organizationId, profileId));
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>Идентификатор пользователя из внешнего Profile Service.</summary>
    public Guid ProfileId { get; private set; }

    /// <summary>Идентификатор кастомной роли участника в организации.</summary>
    public Guid OrganizationMemberRoleId { get; private set; }

    /// <summary>Текущий статус участника.</summary>
    public OrganizationStatus Status { get; private set; }

    /// <summary>Дата начала участия в организации.</summary>
    public DateOnly StartDate { get; private set; }

    /// <summary>Дата окончания участия. null означает, что участие актуально.</summary>
    public DateOnly? EndDate { get; private set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <summary>Изменяет роль участника в организации.</summary>
    public void ChangeRole(Guid organizationMemberRoleId)
    {
        if (organizationMemberRoleId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор роли не может быть пустым.",
                nameof(organizationMemberRoleId)
            );

        OrganizationMemberRoleId = organizationMemberRoleId;
        RegisterDomainEvent(
            new OrganizationMemberRoleChangedDomainEvent(OrganizationId, ProfileId)
        );
    }

    /// <summary>Завершает участие в организации с указанием даты.</summary>
    /// <param name="endDate">Дата окончания участия.</param>
    /// <exception cref="ArgumentException">Если дата окончания раньше даты начала.</exception>
    public void Deactivate(DateOnly endDate)
    {
        if (endDate < StartDate)
        {
            throw new ArgumentException(
                "Дата окончания участия не может быть раньше даты начала.",
                nameof(endDate)
            );
        }

        EndDate = endDate;
        Status = OrganizationStatus.Archived;
        RegisterDomainEvent(
            new OrganizationMemberStatusChangedDomainEvent(OrganizationId, ProfileId)
        );
    }

    /// <inheritdoc />
    public void Delete()
    {
        IsDeleted = true;
        Status = OrganizationStatus.Deleted;
        RegisterDomainEvent(
            new OrganizationMemberStatusChangedDomainEvent(OrganizationId, ProfileId)
        );
    }
}
