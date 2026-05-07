using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Chassis.EventBus.Wolverine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;

namespace Edvantix.Chassis.EventBus;

public static class Extensions
{
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Регистрирует и настраивает инфраструктуру шины событий для текущего хоста,
        /// делегируя вызов WolverineFx через <c>UseWolverineEventFramework</c>. Если
        /// строка подключения к брокеру отсутствует, вызов является холостым.
        /// </summary>
        /// <param name="configure">
        /// Необязательный обратный вызов для дополнительной настройки <see cref="WolverineOptions" />
        /// (например, обнаружение обработчиков, специфичных для сервиса, подключение
        /// Postgres persistence или регистрация слушателей Kafka).
        /// </param>
        public void AddEventBus(Action<WolverineOptions>? configure = null)
        {
            builder.UseEventFramework(configure);
        }
    }

    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует сервис диспетчера событий в контейнере внедрения зависимостей.
        /// </summary>
        /// <remarks>
        /// Диспетчер регистрируется с временем жизни Scoped — один экземпляр на область запроса.
        /// </remarks>
        public void AddEventDispatcher()
        {
            services.AddScoped<IEventDispatcher, EventDispatcher>();
        }
    }
}
