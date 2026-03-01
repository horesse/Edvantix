using Edvantix.Chassis.EF.Contexts;

namespace Edvantix.Catalog.Infrastructure;

/// <summary>
/// Контекст базы данных микросервиса Catalog.
/// Управляет справочными сущностями: валюты, страны, часовые пояса, языки, регионы.
/// </summary>
public sealed class CatalogDbContext(DbContextOptions options) : PostgresContext(options)
{
    /// <summary>Валюты (ISO 4217).</summary>
    public DbSet<Currency> Currencies => Set<Currency>();

    /// <summary>Страны (ISO 3166-1).</summary>
    public DbSet<Country> Countries => Set<Country>();

    /// <summary>Часовые пояса (IANA).</summary>
    public DbSet<Timezone> Timezones => Set<Timezone>();

    /// <summary>Языки (ISO 639-1).</summary>
    public DbSet<Language> Languages => Set<Language>();

    /// <summary>Географические регионы.</summary>
    public DbSet<Region> Regions => Set<Region>();
}
