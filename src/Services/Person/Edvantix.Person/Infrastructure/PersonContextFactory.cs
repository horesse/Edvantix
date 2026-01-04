using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.Person.Infrastructure;

public class PersonContextFactory : IDesignTimeDbContextFactory<PersonContext>
{
    public PersonContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<PersonContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Person"))
            .UseSnakeCaseNamingConvention();
        return new PersonContext(optionsBuilder.Options);
    }
}
