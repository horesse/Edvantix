using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.OrganizationAggregate.Specifications;

public sealed class OrganizationSpecification : CommonSpecification<Organization>
{
    private readonly string? _name;
    private readonly string? _nameLatin;

    public string? Name
    {
        get => _name;
        init
        {
            _name = value;
            ApplyWhereExpressions();
        }
    }

    public string? NameLatin
    {
        get => _nameLatin;
        init
        {
            _nameLatin = value;
            ApplyWhereExpressions();
        }
    }

    private void ApplyWhereExpressions()
    {
        if (!string.IsNullOrWhiteSpace(_name))
            Query.Where(x => x.Name.Contains(_name));

        if (!string.IsNullOrWhiteSpace(_nameLatin))
            Query.Where(x => x.NameLatin.Contains(_nameLatin));
    }
}

