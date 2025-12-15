using Edvantix.Chassis.EF;
using Edvantix.Chassis.Repository;
using Edvantix.Constants.Aspire;

namespace Edvantix.OrganizationManagement.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<OrganizationContext>(
            Components.Database.Organization,
            app =>
            {
                if (app.Environment.IsDevelopment())
                {
                    services.AddMigration<OrganizationContext>();
                }
                else
                {
                    services.AddMigration<OrganizationContext>();
                }

                services.AddRepositories(typeof(IOrganizationApiMarker));
            }
        );
    }
}
