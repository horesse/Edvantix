using Asp.Versioning;
using Edvantix.Constants.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Endpoints;

public static class Extension
{
    extension(IServiceCollection service)
    {
        /// <summary>
        /// Настраивает версионирование API и метаданные API Explorer для обнаружения эндпоинтов.
        /// </summary>
        /// <remarks>
        /// Устанавливает версию API по умолчанию <c>v1</c>, читает версии из сегментов URL
        /// и включает подстановку версий в шаблонах маршрутов для сгруппированной документации API.
        /// </remarks>
        public void AddVersioning()
        {
            service
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = ApiVersions.V1;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'V";
                    options.SubstituteApiVersionInUrl = true;
                });
        }
    }
}
