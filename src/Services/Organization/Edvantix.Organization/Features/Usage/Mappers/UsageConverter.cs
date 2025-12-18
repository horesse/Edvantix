using Edvantix.Chassis.Converter;
using Edvantix.Organization.Features.Usage.Models;

namespace Edvantix.Organization.Features.Usage.Mappers;

public sealed class UsageConverter
    : ClassConverter<UsageModel, Domain.AggregatesModel.UsageAggregate.Usage>
{
    public override Domain.AggregatesModel.UsageAggregate.Usage Map(UsageModel source)
    {
        return new Domain.AggregatesModel.UsageAggregate.Usage(
            source.OrganizationId,
            source.LimitId,
            source.Value
        );
    }

    public override UsageModel Map(Domain.AggregatesModel.UsageAggregate.Usage source)
    {
        return new UsageModel
        {
            Id = source.Id,
            OrganizationId = source.OrganizationId,
            LimitId = source.LimitId,
            Value = source.Value,
        };
    }

    public override void SetProperties(
        UsageModel source,
        Domain.AggregatesModel.UsageAggregate.Usage target
    )
    {
        // Read-only via API, updates only via gRPC
        throw new NotSupportedException(
            "Usage updates via API are not supported. Use gRPC service."
        );
    }
}
