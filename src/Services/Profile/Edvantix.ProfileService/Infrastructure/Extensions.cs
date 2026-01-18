using Edvantix.Chassis.EF;
using Edvantix.Chassis.Repository;
using Edvantix.Constants.Aspire;

namespace Edvantix.ProfileService.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<ProfileContext>(
            Components.Database.Profile,
            app =>
            {
                if (app.Environment.IsDevelopment())
                {
                    services.AddMigration<ProfileContext>();
                }
                else
                {
                    services.AddMigration<ProfileContext>();
                }

                services.AddRepositories(typeof(IProfileApiMarker));
            }
        );
    }
}
