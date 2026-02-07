using Edvantix.Chassis.CQRS.Crud;
using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate.Specifications;
using Edvantix.ProfileService.Features.ProfileFeature.Models;

namespace Edvantix.ProfileService.Features.ProfileFeature;

public static class Extensions
{
    public static IServiceCollection AddPersonInfoFeature(this IServiceCollection services)
    {
        return services;
    }
}
