using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.ContactAggregate.Specifications;

public sealed class ContactSpecification : CommonSpecification<Contact>
{
    private readonly long? _organizationId;
    private readonly ContactType? _type;

    public long? OrganizationId
    {
        get => _organizationId;
        init
        {
            _organizationId = value;
            ApplyWhereExpressions();
        }
    }

    public ContactType? Type
    {
        get => _type;
        init
        {
            _type = value;
            ApplyWhereExpressions();
        }
    }

    private void ApplyWhereExpressions()
    {
        if (_organizationId.HasValue)
            Query.Where(x => x.OrganizationId == _organizationId.Value);

        if (_type.HasValue)
            Query.Where(x => x.Type == _type.Value);
    }
}

