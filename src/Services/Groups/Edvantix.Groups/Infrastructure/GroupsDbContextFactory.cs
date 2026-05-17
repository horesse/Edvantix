using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.Groups.Infrastructure;

/// <summary>
/// Фабрика контекста базы данных для design-time инструментов EF Core (миграции и т.п.).
/// </summary>
public class GroupsDbContextFactory : IDesignTimeDbContextFactory<GroupsDbContext>
{
    /// <inheritdoc />
    public GroupsDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<GroupsDbContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Groups"))
            .UseSnakeCaseNamingConvention();
        return new GroupsDbContext(optionsBuilder.Options);
    }
}
