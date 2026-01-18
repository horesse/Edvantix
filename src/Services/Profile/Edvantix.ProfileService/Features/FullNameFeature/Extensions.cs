using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.ProfileService.CQRS.Extensions;
using Edvantix.ProfileService.Domain.AggregatesModel.FullNameAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.FullNameAggregate.Specifications;
using Edvantix.ProfileService.Features.FullNameFeature.Models;

namespace Edvantix.ProfileService.Features.FullNameFeature;

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
