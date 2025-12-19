using Edvantix.Chassis.CQRS.Crud;
using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.Company.Domain.AggregatesModel.OrganizationAggregate.Specifications;
using Edvantix.Company.Features.Org.Models;
using Edvantix.Constants.Other;

namespace Edvantix.Company.Features.Org;

public static class Extensions
{
    public static IServiceCollection AddOrganizationFeature(this IServiceCollection services)
    {
        services.AddCrudHandlers<
            OrganizationModel,
            long,
            Domain.AggregatesModel.OrganizationAggregate.Organization,
            OrganizationSpecification
        >(CrudActions.ReadOnly);

        services.AddCrudEndpoints<
            Domain.AggregatesModel.OrganizationAggregate.Organization,
            OrganizationModel,
            long,
            OrganizationSpecification
        >(CrudActions.ReadOnly);

        return services;
    }
}
