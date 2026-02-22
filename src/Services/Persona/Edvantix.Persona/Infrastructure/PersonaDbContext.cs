using Edvantix.Chassis.EF.Contexts;

namespace Edvantix.Persona.Infrastructure;

public sealed class PersonaDbContext(DbContextOptions options) : PostgresContext(options);
