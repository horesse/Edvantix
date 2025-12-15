using Edvantix.Chassis.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.OrganizationManagement.Infrastructure;

public sealed class OrganizationContext(DbContextOptions options) : PostgresContext(options);
