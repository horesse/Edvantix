using Edvantix.Chassis.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.System.Infrastructure;

public sealed class SystemContext(DbContextOptions options) : PostgresContext(options);
