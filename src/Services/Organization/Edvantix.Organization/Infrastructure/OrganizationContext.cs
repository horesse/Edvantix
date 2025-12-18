using Edvantix.Chassis.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.Organization.Infrastructure;

public sealed class OrganizationContext(DbContextOptions options) : PostgresContext(options);
