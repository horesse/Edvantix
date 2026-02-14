using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Company.Domain.AggregatesModel.InvitationAggregate.Specifications;

/// <summary>
/// Спецификация для поиска ожидающих приглашений по идентификатору организации.
/// </summary>
public sealed class PendingInvitationsByOrganizationSpecification : CommonSpecification<Invitation>
{
    public PendingInvitationsByOrganizationSpecification(long organizationId)
    {
        Query
            .Where(x => x.OrganizationId == organizationId)
            .Where(x => x.Status == InvitationStatus.Pending);
    }
}
