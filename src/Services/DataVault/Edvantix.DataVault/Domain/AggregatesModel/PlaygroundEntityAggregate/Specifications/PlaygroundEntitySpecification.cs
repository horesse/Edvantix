using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.DataVault.Domain.AggregatesModel.PlaygroundEntityAggregate.Specifications;

public sealed class PlaygroundEntitySpecification : CommonSpecification<PlaygroundEntity>
{
    private readonly decimal? _value;

    public decimal? Value
    {
        get => _value;
        init
        {
            _value = value;
            ApplyWhereExpressions();
        }
    }

    private void ApplyWhereExpressions()
    {
        if (_value.HasValue)
            Query.Where(x => x.Value == _value.Value);
    }
}
