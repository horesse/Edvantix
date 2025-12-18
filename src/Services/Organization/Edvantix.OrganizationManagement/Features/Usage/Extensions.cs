using Edvantix.Chassis.CQRS.Crud;
using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.UsageAggregate.Specifications;
using Edvantix.OrganizationManagement.Features.Usage.Models;

namespace Edvantix.OrganizationManagement.Features.Usage;

public static class Extensions
{
    public static IServiceCollection AddUsageFeature(this IServiceCollection services)
    {
        // Full CRUD handlers and endpoints (read-only enforced via converter and gRPC)
        services.AddCrudHandlers<
            UsageModel,
            long,
            Domain.AggregatesModel.UsageAggregate.Usage,
            UsageSpecification
        >();

        services.AddCrudEndpoints<
            Domain.AggregatesModel.UsageAggregate.Usage,
            UsageModel,
            long,
            UsageSpecification
        >();

        return services;
    }
}

