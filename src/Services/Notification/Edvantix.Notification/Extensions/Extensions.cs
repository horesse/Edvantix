using Edvantix.Chassis.EF;
using Edvantix.Chassis.OpenTelemetry.ActivityScope;
using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.Notification.Infrastructure;
using Edvantix.Notification.Infrastructure.Senders.MailKit;
using Edvantix.Notification.Infrastructure.Senders.Outbox;
using Edvantix.Notification.Infrastructure.Senders.SendGrid;

namespace Edvantix.Notification.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddDefaultCors();

        // Add exception handlers
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

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
