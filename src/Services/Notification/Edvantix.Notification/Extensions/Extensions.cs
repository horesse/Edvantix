using Edvantix.Chassis.EF;
using Edvantix.Chassis.OpenTelemetry.ActivityScope;
using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.Notification.Infrastructure;
using Edvantix.Notification.Infrastructure.Senders.InApp;
using Edvantix.Notification.Infrastructure.Senders.MailKit;
using Edvantix.Notification.Infrastructure.Senders.Outbox;
using Edvantix.Notification.Infrastructure.Senders.SendGrid;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Notification.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        // Аутентификация через Keycloak JWT (тот же flow, что и в других сервисах)
        builder.AddDefaultAuthentication().WithKeycloakClaimsTransformation();

        services
            .AddAuthorizationBuilder()
            .SetDefaultPolicy(
                new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()
            );

        // Add exception handlers
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        builder.AddDefaultOpenApi();

        services.AddSingleton(
            new JsonSerializerOptions { Converters = { DateOnlyJsonConverter.Instance } }
        );

        services.AddSingleton<IActivityScope, ActivityScope>();

        builder.AddAzurePostgresDbContext<NotificationDbContext>(
            Components.Database.Notification,
            _ =>
            {
                services.AddMigration<NotificationDbContext>();

                // Регистрируем репозитории для всех маркированных типов
                services.AddRepositories(typeof(INotificationApiMarker));
            },
            true
        );

        // Отправитель in-app уведомлений (используется MassTransit-потребителем)
        services.AddScoped<IInAppSender, InAppNotificationSender>();

        // Resilience pipeline for the notification service
        builder.AddMailResiliencePipeline();

        services.AddSingleton<IRenderer, MjmlTemplateRenderer>();

        // Register the mailkit sender for development
        // and the sendgrid sender for other environments
        if (builder.Environment.IsDevelopment())
        {
            builder.AddMailKitClient(Components.MailPit);
        }
        else
        {
            builder.AddSendGridClient();
        }

        builder.AddEmailOutbox();

        // Версионированные REST-эндпоинты для фронтенда
        services.AddVersioning();
        services.AddEndpoints(typeof(INotificationApiMarker));

        // Mediator (CQRS) — один обработчик на каждую фичу
        services
            .AddMediator((MediatorOptions options) => options.ServiceLifetime = ServiceLifetime.Scoped)
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(Edvantix.Chassis.CQRS.Pipelines.ActivityBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(Edvantix.Chassis.CQRS.Pipelines.LoggingBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(Edvantix.Chassis.CQRS.Pipelines.ValidationBehavior<,>));

        // Keycloak token introspection middleware (используется в pipeline)
        services.AddScoped<KeycloakTokenIntrospectionMiddleware>();

        builder.AddEventBus(
            typeof(INotificationApiMarker),
            cfg =>
            {
                cfg.AddEntityFrameworkOutbox<NotificationDbContext>(o =>
                {
                    o.QueryDelay = TimeSpan.FromSeconds(1);

                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);

                    o.UsePostgres();

                    o.UseBusOutbox();
                });

                cfg.AddConfigureEndpointsCallback(
                    (context, _, configurator) =>
                        configurator.UseEntityFrameworkOutbox<NotificationDbContext>(context)
                );
            }
        );
    }
}
