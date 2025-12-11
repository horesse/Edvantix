using Edvantix.Chassis.CQRS.Crud;
using Edvantix.Chassis.Endpoints.Crud;
using Edvantix.Chassis.Specification.Generic;
using Edvantix.DataVault.Domain.AggregatesModel.PlaygroundEntityAggregate;
using Edvantix.DataVault.Features.PlaygroundEntityFeature.Models;

namespace Edvantix.DataVault.Features.PlaygroundEntityFeature;

public static class Extensions
{
    public static IServiceCollection AddPlaygroundEntityFeature(this IServiceCollection services)
    {
        services.AddCrudHandlers<
            PlaygroundEntityModel,
            long,
            PlaygroundEntity,
            CommonSpecification<PlaygroundEntity>
        >();
        services.AddCrudEndpoints<
            PlaygroundEntity,
            PlaygroundEntityModel,
            long,
            CommonSpecification<PlaygroundEntity>
        >();

        return services;
    }
}
