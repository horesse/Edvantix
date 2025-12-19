using Edvantix.Chassis.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.Person.Infrastructure;

public sealed class PersonContext(DbContextOptions options) : PostgresContext(options);
