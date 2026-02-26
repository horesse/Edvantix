using Edvantix.Chassis.CQRS.Command;
using Edvantix.Chassis.CQRS.Pipelines;
using Edvantix.Chassis.CQRS.Query;
using Edvantix.Chassis.EF;
using Edvantix.Chassis.OpenTelemetry.ActivityScope;
using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.Notification.Infrastructure;
using Edvantix.Notification.Infrastructure.Senders;
using Edvantix.Notification.Infrastructure.Senders.InApp;
using Edvantix.Notification.Infrastructure.Senders.MailKit;
using Edvantix.Notification.Infrastructure.Senders.Outbox;
using Edvantix.Notification.Infrastructure.Senders.SendGrid;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Notification.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        builder.AddDefaultAuthentication().WithKeycloakClaimsTransformation();

        services
            .AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

        builder.AddDefaultOpenApi();

        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddRateLimiting();

        services.AddSingleton(
            new JsonSerializerOptions { Converters = { DateOnlyJsonConverter.Instance } }
        );

        services.AddSingleton<IActivityScope, ActivityScope>();

        builder.AddAzurePostgresDbContext<NotificationDbContext>(
            Components.Database.Notification,
            _ =>
            {
                services.AddMigration<NotificationDbContext>();

                services.AddRepositories(typeof(INotificationApiMarker));
            },
            true
        );

        services.AddScoped<IInAppSender, InAppNotificationSender>();

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

        services.AddSingleton<IActivityScope, ActivityScope>();
        services.AddSingleton<CommandHandlerMetrics>();
        services.AddSingleton<QueryHandlerMetrics>();

        services.AddVersioning();
        services.AddEndpoints(typeof(INotificationApiMarker));

        services
            .AddMediator(
                (MediatorOptions options) => options.ServiceLifetime = ServiceLifetime.Scoped
            )
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ActivityBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<KeycloakTokenIntrospectionMiddleware>();

        services.AddTransient(s => s.GetRequiredService<IHttpContextAccessor>().HttpContext!.User);

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
