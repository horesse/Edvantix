using Edvantix.Chassis.CQRS.Crud;
using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.Company.Domain.AggregatesModel.UsageAggregate.Specifications;
using Edvantix.Company.Features.Usage.Models;
using Edvantix.Constants.Other;

namespace Edvantix.Company.Features.Usage;

public static class Extensions
{
    public static IServiceCollection AddUsageFeature(this IServiceCollection services)
    {
        services.AddCrudHandlers<
            UsageModel,
            long,
            Domain.AggregatesModel.UsageAggregate.Usage,
            UsageSpecification
        >(CrudActions.ReadOnly);

        services.AddCrudEndpoints<
            Domain.AggregatesModel.UsageAggregate.Usage,
            UsageModel,
            long,
            UsageSpecification
        >(CrudActions.ReadOnly);

        return services;
    }
}
