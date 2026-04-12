using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.Extensions.Configuration;

namespace Edvantix.ServiceDefaults.Cors;

public static class CorsExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Регистрирует политику CORS, соответствующую текущей среде.
        /// В режиме разработки разрешает любые запросы с localhost.
        /// В остальных средах применяет строготипизированную конфигурацию <see cref="CorsSettings" />.
        /// </summary>
        public void AddDefaultCors()
        {
            var services = builder.Services;

            if (builder.Environment.IsDevelopment())
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(
                        CorsConstants.AllowAllCorsPolicy,
                        policyBuilder =>
                        {
                            policyBuilder
                                .SetIsOriginAllowed(origin =>
                                    new Uri(origin).Host == Network.Localhost
                                )
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                        }
                    );
                });
            }
            else
            {
                builder.Configure<CorsSettings>(CorsSettings.ConfigurationSection);

                var corsSettings =
                    builder
                        .Configuration.GetRequiredSection(CorsSettings.ConfigurationSection)
                        .Get<CorsSettings>()
                    ?? throw new InvalidOperationException(
                        $"Failed to bind CORS settings from configuration section: {CorsSettings.ConfigurationSection}"
                    );

                services.AddCors(options =>
                {
                    options.AddPolicy(
                        CorsConstants.AllowSpecificCorsPolicy,
                        policyBuilder =>
                        {
                            policyBuilder
                                .WithOrigins([.. corsSettings.Origins])
                                .WithHeaders([.. corsSettings.Headers])
                                .WithMethods([.. corsSettings.Methods]);

                            if (corsSettings.MaxAge is not null)
                            {
                                policyBuilder.SetPreflightMaxAge(
                                    TimeSpan.FromSeconds(corsSettings.MaxAge.Value)
                                );
                            }

                            if (corsSettings.AllowCredentials)
                            {
                                policyBuilder.AllowCredentials();
                            }
                        }
                    );
                });
            }
        }
    }

    extension(WebApplication app)
    {
        /// <summary>
        /// Применяет CORS middleware, используя политику, зарегистрированную в <see cref="CorsExtensions" />.
        /// Выбирает <c>AllowAll</c> в режиме разработки и <c>AllowSpecific</c> во всех остальных средах.
        /// </summary>
        public void UseDefaultCors()
        {
            app.UseCors(
                app.Environment.IsDevelopment()
                    ? CorsConstants.AllowAllCorsPolicy
                    : CorsConstants.AllowSpecificCorsPolicy
            );
        }
    }
}
