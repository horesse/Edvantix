using Edvantix.Chassis.EF;
using Edvantix.Chassis.Repository;
using Edvantix.Constants.Aspire;

namespace Edvantix.EntityHub.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder, bool isFullFeatures = true)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<EntityHubContext>(
            Components.Database.EntityHub,
            app =>
            {
                if (isFullFeatures)
                {
                    services.AddMigration<EntityHubContext>();
                }
                
                services.AddRepositories(typeof(IEntityHubApiMarker));
            },
            excludeDefaultInterceptors: !isFullFeatures
        );
    }
}
