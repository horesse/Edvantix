using Edvantix.Chassis.EF;
using Edvantix.Chassis.Repository;
using Edvantix.Constants.Aspire;

namespace Edvantix.System.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<SystemContext>(
            Components.Database.System,
            app =>
            {
                if (app.Environment.IsDevelopment())
                {
                    services.AddMigration<SystemContext>();
                }
                else
                {
                    services.AddMigration<SystemContext>();
                }

                services.AddRepositories(typeof(ISystemApiMarker));
            }
        );
    }
}
