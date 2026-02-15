using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Company.Domain.AggregatesModel.InvitationAggregate.Specifications;

/// <summary>
/// Спецификация для проверки дублирующих ожидающих приглашений
/// (по email или profileId в рамках одной организации).
/// </summary>
public sealed class DuplicatePendingInvitationSpecification : CommonSpecification<Invitation>
{
    public DuplicatePendingInvitationSpecification(
        long organizationId,
        string? email = null,
        long? profileId = null
    )
    {
        Query
            .Where(x => x.OrganizationId == organizationId)
            .Where(x => x.Status == InvitationStatus.Pending);

        if (email is not null)
            Query.Where(x => x.InviteeEmail == email);

        if (profileId.HasValue)
            Query.Where(x => x.InviteeProfileId == profileId.Value);
    }
}
