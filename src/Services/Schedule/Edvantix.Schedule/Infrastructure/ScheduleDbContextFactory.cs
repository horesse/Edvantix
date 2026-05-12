using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.Schedule.Infrastructure;

public class ScheduleDbContextFactory : IDesignTimeDbContextFactory<ScheduleDbContext>
{
    public ScheduleDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ScheduleDbContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Schedule"))
            .UseSnakeCaseNamingConvention();
        return new ScheduleDbContext(optionsBuilder.Options);
    }
}
