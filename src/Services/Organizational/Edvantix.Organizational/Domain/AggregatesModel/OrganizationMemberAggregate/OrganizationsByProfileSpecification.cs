using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>
/// Спецификация для получения активных членств пользователя во всех организациях,
/// включая роль участника.
/// </summary>
public sealed class OrganizationsByProfileSpecification : Specification<OrganizationMember>
{
    public OrganizationsByProfileSpecification(Guid profileId)
    {
        Query
            .Where(m =>
                m.ProfileId == profileId && m.Status == OrganizationStatus.Active && !m.IsDeleted
            )
            .Include(m => m.Role)
            .OrderByDescending(m => m.StartDate);
    }
}
