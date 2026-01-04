using Edvantix.Chassis.CQRS.Crud;
using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;
using Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate.Specifications;
using Edvantix.Person.Features.PersonInfoFeature.Models;

namespace Edvantix.Person.Features.PersonInfoFeature;

public static class Extensions
{
    public static IServiceCollection AddPersonInfoFeature(this IServiceCollection services)
    {
        services.AddCrudHandlers<PersonInfoModel, long, PersonInfo, PersonInfoSpecification>();

        services.AddCrudEndpoints<PersonInfo, PersonInfoModel, long, PersonInfoSpecification>();

        return services;
    }
}
