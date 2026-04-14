using Edvantix.Chassis.OpenTelemetry.ActivityScope;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.OpenTelemetry;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует сервис области трассировки в контейнере зависимостей.
        /// </summary>
        /// <returns>Обновлённый экземпляр <see cref="IServiceCollection" />.</returns>
        public IServiceCollection AddActivityScope()
        {
            services.AddSingleton<IActivityScope, ActivityScope.ActivityScope>();
            return services;
        }
    }
}
