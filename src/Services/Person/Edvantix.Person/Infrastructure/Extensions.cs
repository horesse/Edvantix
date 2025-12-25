using Edvantix.Chassis.EF;
using Edvantix.Chassis.Repository;
using Edvantix.Constants.Aspire;

namespace Edvantix.Person.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<PersonContext>(
            Components.Database.Organization,
            app =>
            {
                if (app.Environment.IsDevelopment())
                {
                    services.AddMigration<PersonContext>();
                }
                else
                {
                    services.AddMigration<PersonContext>();
                }

                services.AddRepositories(typeof(IPersonApiMarker));
            }
        );
    }
}
