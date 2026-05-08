using Edvantix.Chassis.EF;
using Edvantix.Chassis.EventBus.Wolverine;
using Edvantix.Chassis.OpenTelemetry;
using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Utilities.Converters;
using Edvantix.Notification.Infrastructure;
using Edvantix.Notification.Infrastructure.Senders.InApp;
using Edvantix.Notification.Infrastructure.Senders.MailKit;
using Edvantix.Notification.Infrastructure.Senders.Outbox;
using Edvantix.Notification.Infrastructure.Senders.SendGrid;
using Edvantix.ServiceDefaults.Cors;

namespace Edvantix.Notification.Extensions;

internal static class Extensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddApplicationServices()
        {
            var services = builder.Services;

            builder.AddDefaultCors();

            services.AddGlobalExceptionHandler();
            services.AddProblemDetails();

            services.AddSingleton(
                new JsonSerializerOptions { Converters = { DateOnlyJsonConverter.Instance } }
            );

            services.AddActivityScope();

            builder.AddAzurePostgresDbContext<NotificationDbContext>(
                Components.Database.Notification,
                _ =>
                {
                    services.AddMigration<NotificationDbContext>();

                    services.AddRepositories(typeof(INotificationApiMarker));
                },
                true
            );

            builder.AddMailResiliencePipeline();

            services.AddSingleton<IRenderer, MjmlTemplateRenderer>();

            if (builder.Environment.IsDevelopment())
            {
                builder.AddMailKitClient(Components.MailPit);
            }
            else
            {
                builder.AddSendGridClient();
            }

            builder.AddEmailOutbox();
            builder.AddInAppSender();

            builder.AddEventBus(opts =>
            {
                opts.Discovery.IncludeAssembly(typeof(INotificationApiMarker).Assembly);
                opts.ListenToIntegrationEventsIn(typeof(INotificationApiMarker).Assembly);
            });
        }
    }
}
