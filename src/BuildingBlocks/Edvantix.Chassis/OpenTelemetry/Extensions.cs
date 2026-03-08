using Edvantix.Chassis.OpenTelemetry.ActivityScope;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.OpenTelemetry;

public static class Extensions
{
    public static IServiceCollection AddActivityScope(this IServiceCollection services)
    {
        services.AddSingleton<IActivityScope, ActivityScope.ActivityScope>();
        return services;
    }
}
