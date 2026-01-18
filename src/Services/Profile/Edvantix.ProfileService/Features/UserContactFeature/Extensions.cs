using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.ProfileService.CQRS.Extensions;
using Edvantix.ProfileService.Domain.AggregatesModel.ContactAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.ContactAggregate.Specifications;
using Edvantix.ProfileService.Features.UserContactFeature.Models;

namespace Edvantix.ProfileService.Features.UserContactFeature;

public static class Extensions
{
    public static IServiceCollection AddContactFeature(this IServiceCollection services)
    {
        services.AddPersonalDataCrudHandlers<
            UserContactModel,
            long,
            UserContact,
            UserContactSpecification
        >();

        services.AddCrudEndpoints<UserContact, UserContactModel, long, UserContactSpecification>();

        return services;
    }
}
