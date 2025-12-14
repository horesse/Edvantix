using Microsoft.EntityFrameworkCore;

namespace Edvantix.Chassis.EF.Contexts;

public abstract class PostgresContext(DbContextOptions options) : DatabaseContext(options);
