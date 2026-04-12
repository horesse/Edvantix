using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Microsoft.AspNetCore.OpenApi;

namespace Edvantix.ServiceDefaults.ApiSpecification.OpenApi;

public static class OpenApiExtensions
{
    extension(WebApplication app)
    {
        /// <summary>
        /// Регистрирует эндпоинты OpenAPI для локальной разработки.
        /// </summary>
        /// <remarks>
        /// Намеренно включается только в среде разработки во избежание экспозиции спецификации API в других окружениях.
        /// </remarks>
        public void UseDefaultOpenApi()
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapGet("/", () => TypedResults.Redirect("openapi/v1.json"))
                    .ExcludeFromDescription();
            }
        }
    }

    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует конфигурацию OpenAPI по умолчанию для сервиса.
        /// </summary>
        /// <param name="configure">
        /// Необязательный обратный вызов для дополнительной настройки <see cref="OpenApiOptions" /> после применения трансформеров по умолчанию.
        /// </param>
        /// <remarks>
        /// Добавляет метаданные авторизации и определения схем безопасности для единообразия документации API между сервисами.
        /// </remarks>
        public void AddDefaultOpenApi(Action<OpenApiOptions>? configure = null)
        {
            services.AddOpenApi(options =>
            {
                options.AddOperationTransformer<AuthorizationChecksTransformer>();
                options.AddDocumentTransformer<SecuritySchemeDefinitionsTransformer>();
                configure?.Invoke(options);
            });
        }
    }
}
