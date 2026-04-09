using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.Persona.Infrastructure;

public class PersonaDbContextFactory : IDesignTimeDbContextFactory<PersonaDbContext>
{
    public PersonaDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<PersonaDbContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Persona"))
            .UseSnakeCaseNamingConvention();
        return new PersonaDbContext(optionsBuilder.Options);
    }
}
