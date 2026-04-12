using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edvantix.Chassis.Endpoints;

public static class RegisterEndpointExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Сканирует сборку, содержащую указанный тип, и регистрирует все конкретные реализации
        /// <see cref="IEndpoint" /> в контейнере зависимостей.
        /// </summary>
        /// <param name="type">
        /// Маркерный тип для поиска целевой сборки при обнаружении эндпоинтов.
        /// </param>
        public void AddEndpoints(Type type)
        {
            services.Scan(scan =>
                scan.FromAssembliesOf(type)
                    .AddClasses(
                        classes =>
                            classes
                                .AssignableTo<IEndpoint>()
                                .Where(typeInfo =>
                                    typeInfo is { IsAbstract: false, IsInterface: false }
                                ),
                        false
                    )
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
            );
        }
    }

    extension(WebApplication app)
    {
        /// <summary>
        /// Регистрирует все обнаруженные реализации <see cref="IEndpoint" /> в версионированной группе маршрутов API.
        /// </summary>
        /// <param name="apiVersionSet">
        /// Набор версий API, применяемый к группе эндпоинтов.
        /// </param>
        /// <param name="resourceName">
        /// Необязательный сегмент ресурса, добавляемый к базовому маршруту.
        /// Если <see langword="null" /> или пустой, используется только версионированный базовый путь.
        /// </param>
        public void MapEndpoints(ApiVersionSet apiVersionSet, string? resourceName = null)
        {
            using var scope = app.Services.CreateScope();

            var endpoints = scope.ServiceProvider.GetRequiredService<IEnumerable<IEndpoint>>();

            var builder = app.MapGroup(
                    resourceName is null or { Length: 0 }
                        ? "/api/v{version:apiVersion}"
                        : $"/api/v{{version:apiVersion}}/{resourceName}"
                )
                .WithApiVersionSet(apiVersionSet);

            if (app.Environment.IsDevelopment())
            {
                builder.DisableAntiforgery();
            }

            foreach (var endpoint in endpoints)
            {
                endpoint.MapEndpoint(builder);
            }
        }
    }
}
