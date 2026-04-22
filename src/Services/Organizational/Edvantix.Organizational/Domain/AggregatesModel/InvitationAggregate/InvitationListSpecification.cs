using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

/// <summary>Постраничный список приглашений организации с опциональной фильтрацией по статусу.</summary>
public sealed class InvitationListSpecification : Specification<Invitation>
{
    public InvitationListSpecification(
        Guid organizationId,
        int offset,
        int limit,
        InvitationStatus? status = null
    )
    {
        Query
            .AsNoTracking()
            .Where(i => i.OrganizationId == organizationId && !i.IsDeleted)
            .OrderByDescending(i => i.CreatedAt)
            .Skip(offset)
            .Take(limit);

        if (status.HasValue)
            Query.Where(i => i.Status == status.Value);
    }
}
