using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Company.Domain.AggregatesModel.InvitationAggregate.Specifications;

/// <summary>
/// Спецификация для поиска ожидающих приглашений по идентификатору профиля приглашённого.
/// </summary>
public sealed class InvitationsByInviteeProfileSpecification : CommonSpecification<Invitation>
{
    public InvitationsByInviteeProfileSpecification(long profileId)
    {
        Query
            .Where(x => x.InviteeProfileId == profileId)
            .Where(x => x.Status == InvitationStatus.Pending);
    }
}
