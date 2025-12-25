using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.Person.Domain.AggregatesModel.EmploymentHistoryAggregate;
using Edvantix.Person.Domain.AggregatesModel.EmploymentHistoryAggregate.Specifications;
using Edvantix.Person.Extensions;
using Edvantix.Person.Features.EmploymentHistoryFeature.Models;

namespace Edvantix.Person.Features.EmploymentHistoryFeature;

public static class Extensions
{
    public static IServiceCollection AddEmploymentHistoryFeature(this IServiceCollection services)
    {
        services.AddPersonalDataCrudHandlers<
            EmploymentHistoryModel,
            long,
            EmploymentHistory,
            EmploymentHistorySpecification
        >();

        services.AddCrudEndpoints<
            EmploymentHistory,
            EmploymentHistoryModel,
            long,
            EmploymentHistorySpecification
        >();

        return services;
    }
}
