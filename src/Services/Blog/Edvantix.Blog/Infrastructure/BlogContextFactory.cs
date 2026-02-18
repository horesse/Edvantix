using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.Blog.Infrastructure;

/// <summary>
/// Фабрика контекста для создания миграций через EF Core CLI.
/// Используется только в режиме design-time при выполнении команд dotnet ef migrations.
/// </summary>
public class BlogContextFactory : IDesignTimeDbContextFactory<BlogContext>
{
    public BlogContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<BlogContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Blog"))
            .UseSnakeCaseNamingConvention();

        return new BlogContext(optionsBuilder.Options);
    }
}
