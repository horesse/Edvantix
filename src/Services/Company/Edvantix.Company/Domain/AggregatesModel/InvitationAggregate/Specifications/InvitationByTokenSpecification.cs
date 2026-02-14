using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Company.Domain.AggregatesModel.InvitationAggregate.Specifications;

/// <summary>
/// Спецификация для поиска приглашения по токену.
/// </summary>
public sealed class InvitationByTokenSpecification : CommonSpecification<Invitation>
{
    public InvitationByTokenSpecification(Guid token)
    {
        Query.Where(x => x.Token == token);
    }
}
