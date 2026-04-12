using System.Diagnostics;
using Edvantix.Chassis.Logging;
using Edvantix.Chassis.OpenTelemetry;
using Edvantix.Chassis.OpenTelemetry.ActivityScope;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Edvantix.ServiceDefaults;

// Регистрирует общие сервисы Aspire: обнаружение сервисов, устойчивость, проверки работоспособности и OpenTelemetry.
// Этот проект должен быть подключён к каждому сервисному проекту решения.
public static class Extensions
{
    extension(IHostApplicationBuilder builder)
    {
        private void AddDefaultHealthChecks()
        {
            builder
                .Services.AddHealthChecks()
                // Добавляет базовую проверку живости для подтверждения работоспособности приложения
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
        }
    }

    extension(WebApplication app)
    {
        /// <summary>
        /// Регистрирует эндпоинты проверки работоспособности только для среды разработки.
        /// </summary>
        /// <remarks>
        /// Эндпоинт готовности требует прохождения всех проверок, эндпоинт живости — только проверок с тегом <c>live</c>.
        /// </remarks>
        public void MapDefaultEndpoints()
        {
            if (!app.Environment.IsDevelopment())
            {
                return;
            }

            // Все проверки работоспособности должны пройти, чтобы приложение считалось готовым принимать трафик.
            app.MapHealthChecks(Http.Endpoints.HealthEndpointPath);

            // Только проверки с тегом "live" должны пройти, чтобы приложение считалось живым.
            app.MapHealthChecks(
                Http.Endpoints.AlivenessEndpointPath,
                new() { Predicate = r => r.Tags.Contains("live") }
            );
        }
    }

    extension(IHostApplicationBuilder builder)
    {
        private void AddLogging()
        {
            var logger = builder.Logging;

            logger.EnableEnrichment();
            builder.AddApplicationEnricher();

            logger.AddGlobalBuffer(builder.Configuration.GetSection("Logging"));
            logger.AddPerIncomingRequestBuffer(builder.Configuration.GetSection("Logging"));

            logger.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            if (builder.Environment.IsDevelopment())
            {
                logger.AddTraceBasedSampler();
            }
        }
    }

    extension(IServiceCollection services)
    {
        private void AddOpenTelemetry(IHostApplicationBuilder builder)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            services
                .AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddMeter(ActivitySourceProvider.DefaultSourceName);
                })
                .WithTracing(tracing =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        tracing.SetSampler(new AlwaysOnSampler());
                    }

                    tracing
                        .AddSource(builder.Environment.ApplicationName)
                        .AddAspNetCoreInstrumentation(options =>
                            // Исключает запросы к эндпоинтам проверки работоспособности из трассировки во избежание шума на дашборде
                            options.Filter = httpContext =>
                                !(
                                    httpContext.Request.Path.StartsWithSegments(
                                        Http.Endpoints.HealthEndpointPath
                                    )
                                    || httpContext.Request.Path.StartsWithSegments(
                                        Http.Endpoints.AlivenessEndpointPath
                                    )
                                )
                        )
                        .AddGrpcClientInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddProcessor(new FixHttpRouteProcessor())
                        .AddSource(ActivitySourceProvider.DefaultSourceName);
                });
        }
    }

    extension<TBuilder>(TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        /// <summary>
        /// Настраивает базовые возможности платформы для хоста сервиса.
        /// </summary>
        /// <remarks>
        /// Включает OpenTelemetry, базовые проверки работоспособности, обнаружение сервисов и устойчивость HTTP-клиентов.
        /// </remarks>
        public void AddServiceDefaults()
        {
            builder.ConfigureOpenTelemetry();

            builder.AddDefaultHealthChecks();

            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                http.RemoveAllResilienceHandlers();

                // Включает устойчивость по умолчанию
                http.AddStandardResilienceHandler();

                // Включает обнаружение сервисов по умолчанию
                http.AddServiceDiscovery();
            });
        }

        private void ConfigureOpenTelemetry()
        {
            var services = builder.Services;

            services.AddHttpContextAccessor();

            builder.AddLogging();

            services.AddOpenTelemetry(builder);

            builder.AddOpenTelemetryExporters();
        }

        private void AddOpenTelemetryExporters()
        {
            var useOtlpExporter = !string.IsNullOrWhiteSpace(
                builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
            );

            if (useOtlpExporter)
            {
                builder.Services.AddOpenTelemetry().UseOtlpExporter();
            }
        }
    }
}
