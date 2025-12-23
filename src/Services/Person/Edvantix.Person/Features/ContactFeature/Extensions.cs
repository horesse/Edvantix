using Edvantix.Chassis.CQRS.Crud;
using Edvantix.Person.Domain.AggregatesModel.ContactAggregate;
using Edvantix.Person.Domain.AggregatesModel.ContactAggregate.Specifications;
using Edvantix.Person.Features.ContactFeature.Models;

namespace Edvantix.Person.Features.ContactFeature;

public static class Extensions
{
    public static IServiceCollection AddContactFeature(this IServiceCollection services)
    {
        services.AddCrudViewModelHandlers<
            ContactModel,
            ContactCreateViewModel,
            ContactModel,
            long,
            Contact,
            ContactSpecification
        >();

        services.AddCrudViewModelHandlers<
            ContactModel,
            ContactCreateViewModel,
            ContactModel,
            long,
            Contact,
            ContactSpecification
        >();

        return services;
    }
}
