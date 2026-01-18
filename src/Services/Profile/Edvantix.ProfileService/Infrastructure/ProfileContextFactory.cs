using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.ProfileService.Infrastructure;

public class ProfileContextFactory : IDesignTimeDbContextFactory<ProfileContext>
{
    public ProfileContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ProfileContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Profile"))
            .UseSnakeCaseNamingConvention();
        return new ProfileContext(optionsBuilder.Options);
    }
}
