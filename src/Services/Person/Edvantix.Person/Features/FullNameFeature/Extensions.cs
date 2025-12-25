using Edvantix.Chassis.CQRS.Crud;
using Edvantix.Person.Domain.AggregatesModel.EmploymentHistoryAggregate;
using Edvantix.Person.Domain.AggregatesModel.EmploymentHistoryAggregate.Specifications;
using Edvantix.Person.Domain.AggregatesModel.FullNameAggregate;
using Edvantix.Person.Domain.AggregatesModel.FullNameAggregate.Specifications;
using Edvantix.Person.Extensions;
using Edvantix.Person.Features.EmploymentHistoryFeature.Models;
using Edvantix.Person.Features.FullNameFeature.Models;

namespace Edvantix.Person.Features.FullNameFeature;

public static class Extensions
{
    public static IServiceCollection AddFullNameFeature(this IServiceCollection services)
    {
        services.AddPersonalDataCrudHandlers<
            FullNameModel,
            long,
            FullName,
            FullNameSpecification
        >();

        services.AddCrudHandlers<
            EmploymentHistoryModel,
            long,
            EmploymentHistory,
            EmploymentHistorySpecification
        >();

        return services;
    }
}
