using Edvantix.Chassis.EF;
using Edvantix.Chassis.Repository;
using Edvantix.Constants.Aspire;

namespace Edvantix.DataVault.Infrastructure;

public static class Extensions
{
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        
        builder.AddAzurePostgresDbContext<DataVaultContext>(
            Components.Database.DataVault,
            app =>
            {
                if (app.Environment.IsDevelopment())
                {
                    services.AddMigration<DataVaultContext>();
                }
                else
                {
                    services.AddMigration<DataVaultContext>();
                }

                services.AddRepositories(typeof(IDataVaultApiMarker));
            }
        );
    }
}
