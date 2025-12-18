using Edvantix.Chassis.CQRS.Crud;
using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.ContactAggregate.Specifications;
using Edvantix.OrganizationManagement.Features.Contact.Models;

namespace Edvantix.OrganizationManagement.Features.Contact;

public static class Extensions
{
    public static IServiceCollection AddContactFeature(this IServiceCollection services)
    {
        services.AddCrudHandlers<
            ContactModel,
            long,
            Domain.AggregatesModel.ContactAggregate.Contact,
            ContactSpecification
        >();

        services.AddCrudEndpoints<
            Domain.AggregatesModel.ContactAggregate.Contact,
            ContactModel,
            long,
            ContactSpecification
        >();

        return services;
    }
}
