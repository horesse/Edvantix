using Edvantix.Chassis.OpenTelemetry.ActivityScope;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.OpenTelemetry;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddActivityScope()
        {
            services.AddSingleton<IActivityScope, ActivityScope.ActivityScope>();
            return services;
        }
    }
}
