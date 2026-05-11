using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.Curriculum.Infrastructure;

public class CurriculumDbContextFactory : IDesignTimeDbContextFactory<CurriculumDbContext>
{
    public CurriculumDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<CurriculumDbContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Curriculum"))
            .UseSnakeCaseNamingConvention();
        return new CurriculumDbContext(optionsBuilder.Options);
    }
}
