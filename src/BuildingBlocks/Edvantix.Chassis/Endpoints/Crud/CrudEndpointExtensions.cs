using Edvantix.Chassis.Endpoints.Crud.Commands;
using Edvantix.Chassis.Endpoints.Crud.Queries;
using Edvantix.SharedKernel.SeedWork;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Endpoints.Crud;

public static class CrudEndpointExtensions
{
    /// <summary>
    /// Регистрирует все стандартные CRUD endpoints для модели
    /// </summary>
    public static IServiceCollection AddCrudEndpoints<TModel, TIdentity>(
        this IServiceCollection services)
        where TModel : Model<TIdentity>
        where TIdentity : struct
    {
        services.AddTransient<IEndpoint, GetByIdEndpoint<TModel, TIdentity>>();
        services.AddTransient<IEndpoint, GetAllEndpoint<TModel, TIdentity>>();
        services.AddTransient<IEndpoint, GetCountEndpoint<TModel, TIdentity>>();
        services.AddTransient<IEndpoint, IsExistEndpoint<TModel, TIdentity>>();
        services.AddTransient<IEndpoint, CreateEndpoint<TModel, TIdentity>>();
        services.AddTransient<IEndpoint, CreateRangeEndpoint<TModel, TIdentity>>();
        services.AddTransient<IEndpoint, UpdateEndpoint<TModel, TIdentity>>();
        services.AddTransient<IEndpoint, UpdateRangeEndpoint<TModel, TIdentity>>();
        services.AddTransient<IEndpoint, DeleteEndpoint<TModel, TIdentity>>();
        services.AddTransient<IEndpoint, DeleteRangeEndpoint<TModel, TIdentity>>();

        return services;
    }

    /// <summary>
    /// Регистрирует кастомный endpoint, переопределяя стандартный
    /// </summary>
    public static IServiceCollection AddCrudEndpoint<TEndpoint>(
        this IServiceCollection services)
        where TEndpoint : class, IEndpoint
    {
        // Удаляем существующую регистрацию если есть
        var descriptor = services.FirstOrDefault(d => 
            d.ServiceType == typeof(IEndpoint) && 
            d.ImplementationType == typeof(TEndpoint));
        
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        services.AddTransient<IEndpoint, TEndpoint>();
        return services;
    }
}
