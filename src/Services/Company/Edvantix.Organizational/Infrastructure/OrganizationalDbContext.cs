using Edvantix.Chassis.EF.Contexts;

namespace Edvantix.Organizational.Infrastructure;

public sealed class OrganizationalDbContext(DbContextOptions options) : PostgresContext(options);
