using Edvantix.Chassis.CQRS.Crud;
using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.Person.Domain.AggregatesModel.ContactAggregate;
using Edvantix.Person.Domain.AggregatesModel.ContactAggregate.Specifications;
using Edvantix.Person.Extensions;
using Edvantix.Person.Features.ContactFeature.Models;

namespace Edvantix.Person.Features.ContactFeature;

public static class Extensions
{
    public static IServiceCollection AddContactFeature(this IServiceCollection services)
    {
        services.AddPersonalDataCrudHandlers<ContactModel, long, Contact, ContactSpecification>();

        services.AddCrudEndpoints<Contact, ContactModel, long, ContactSpecification>();

        return services;
    }
}
