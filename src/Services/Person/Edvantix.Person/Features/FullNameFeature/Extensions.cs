using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.Person.CQRS.Extensions;
using Edvantix.Person.Domain.AggregatesModel.FullNameAggregate;
using Edvantix.Person.Domain.AggregatesModel.FullNameAggregate.Specifications;
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

        services.AddCrudEndpoints<FullName, FullNameModel, long, FullNameSpecification>();

        return services;
    }
}
