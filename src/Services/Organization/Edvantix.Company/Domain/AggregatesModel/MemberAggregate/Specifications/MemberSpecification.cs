using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Company.Domain.AggregatesModel.MemberAggregate.Specifications;

public sealed class MemberSpecification : CommonSpecification<Member>
{
    private readonly long? _organizationId;
    private readonly Guid? _personId;

    public long? OrganizationId
    {
        get => _organizationId;
        init
        {
            _organizationId = value;
            ApplyWhereExpressions();
        }
    }

    public Guid? PersonId
    {
        get => _personId;
        init
        {
            _personId = value;
            ApplyWhereExpressions();
        }
    }

    private void ApplyWhereExpressions()
    {
        if (_organizationId.HasValue)
            Query.Where(x => x.OrganizationId == _organizationId.Value);

        if (_personId.HasValue)
            Query.Where(x => x.PersonId == _personId.Value);
    }
}
