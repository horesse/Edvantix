using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.DataVault.Infrastructure;

public class DataVaultContextFactory: IDesignTimeDbContextFactory<DataVaultContext>
{
    public DataVaultContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<DataVaultContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("DataVault"))
            .UseSnakeCaseNamingConvention();
        return new DataVaultContext(optionsBuilder.Options);
    }
}
