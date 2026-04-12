using Edvantix.Chassis.Security.Settings;
using Edvantix.Chassis.Utilities;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Constants.Aspire;
using Edvantix.Constants.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edvantix.Chassis.Security.Extensions;

public static class AuthenticationExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Настраивает стандартный конвейер аутентификации JWT bearer с использованием параметров Keycloak,
        /// полученных из настроенных identity-опций.
        /// </summary>
        /// <remarks>
        /// Метод также регистрирует именованный HTTP-клиент для Keycloak и включает более строгую
        /// валидацию токенов вне среды разработки.
        /// </remarks>
        /// <returns>Тот же экземпляр <see cref="IHostApplicationBuilder" /> для цепочки вызовов.</returns>
        public IHostApplicationBuilder AddDefaultAuthentication()
        {
            var services = builder.Services;

            // Привязывает секцию конфигурации identity к <see cref="IdentityOptions" />.
            builder.Configure<IdentityOptions>(IdentityOptions.ConfigurationSection);

            // Получает realm Keycloak из привязанных identity-опций.
            var realm = services.BuildServiceProvider().GetRequiredService<IdentityOptions>().Realm;

            // Использует HTTP в среде разработки и HTTP/HTTPS вне её.
            var scheme = builder.Environment.IsDevelopment()
                ? Uri.UriSchemeHttp
                : Http.Schemes.HttpOrHttps;

            // Формирует базовый URL Keycloak на основе внутренних соглашений об именовании компонентов.
            var keycloakUrl = HttpUtilities
                .AsUrlBuilder()
                .WithScheme(scheme)
                .WithHost(Components.KeyCloak)
                .Build();

            // Регистрирует именованный HTTP-клиент для взаимодействия с Keycloak.
            services.AddHttpClient(
                Components.KeyCloak,
                client => client.BaseAddress = new(keycloakUrl)
            );

            // Настраивает JWT bearer-аутентификацию с использованием Keycloak.
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddKeycloakJwtBearer(
                    Components.KeyCloak,
                    realm,
                    options =>
                    {
                        // Использует audience клиента account в Keycloak.
                        options.Audience = "account";
                        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                        options.TokenValidationParameters.ValidateAudience =
                            !builder.Environment.IsDevelopment();
                        options.TokenValidationParameters.ValidateIssuer =
                            !builder.Environment.IsDevelopment();
                    }
                );

            return builder;
        }
    }
}
