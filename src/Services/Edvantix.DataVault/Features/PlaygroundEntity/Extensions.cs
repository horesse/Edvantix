using Edvantix.Chassis.CQRS.Crud;
using Edvantix.DataVault.Features.PlaygroundEntity.Models;
using Edvantix.Chassis.Endpoints.Crud;

namespace Edvantix.DataVault.Features.PlaygroundEntity;

public static class Extensions
{
    public static IServiceCollection AddPlaygroundEntityFeature(this IServiceCollection services)
    {
        services.AddCrudHandlers<PlaygroundEntityModel, long, Domain.AggregatesModel.PlaygroundEntityAggregate.PlaygroundEntity>();
        services.AddCrudEndpoints<PlaygroundEntityModel, long>();
        
        return services;
    }
}
