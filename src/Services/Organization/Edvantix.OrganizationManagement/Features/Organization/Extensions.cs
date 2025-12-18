using Edvantix.Chassis.CQRS.Crud;
using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.OrganizationAggregate.Specifications;
using Edvantix.OrganizationManagement.Features.Organization.Models;

namespace Edvantix.OrganizationManagement.Features.Organization;

public static class Extensions
{
    public static IServiceCollection AddOrganizationFeature(this IServiceCollection services)
    {
        // Full CRUD handlers and endpoints (read-only enforced via converter)
        services.AddCrudHandlers<
            OrganizationModel,
            long,
            Domain.AggregatesModel.OrganizationAggregate.Organization,
            OrganizationSpecification
        >();

        services.AddCrudEndpoints<
            Domain.AggregatesModel.OrganizationAggregate.Organization,
            OrganizationModel,
            long,
            OrganizationSpecification
        >();

        return services;
    }
}

