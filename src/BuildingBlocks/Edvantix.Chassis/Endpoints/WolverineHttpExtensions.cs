using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Http;

namespace Edvantix.Chassis.Endpoints;

public static class WolverineHttpExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует WolverineHttp в контейнере зависимостей.
        /// Должен вызываться вместе с <see cref="AddWolverine{TMarker}"/> из <c>Edvantix.Chassis.CQRS</c>.
        /// </summary>
        public IServiceCollection AddWolverineHttp()
        {
            WolverineHttpEndpointRouteBuilderExtensions.AddWolverineHttp(services);
            return services;
        }
    }

    extension(IEndpointRouteBuilder app)
    {
        /// <summary>
        /// Регистрирует все Wolverine HTTP-эндпоинты (классы с атрибутами <c>[WolverineGet]</c>, <c>[WolverinePost]</c> и т.д.).
        /// Включает eager-прогрев маршрутов и позволяет дополнительно настроить <see cref="WolverineHttpOptions"/>.
        /// </summary>
        /// <param name="configure">Дополнительная конфигурация, например авторизация, tenant ID, политики.</param>
        public IEndpointRouteBuilder MapWolverineApi(Action<WolverineHttpOptions>? configure = null)
        {
            app.MapWolverineEndpoints(opts =>
            {
                opts.WarmUpRoutes = RouteWarmup.Eager;

                configure?.Invoke(opts);
            });

            return app;
        }
    }
}
