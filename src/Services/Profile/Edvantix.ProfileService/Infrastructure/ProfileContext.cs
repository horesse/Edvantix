using Edvantix.Chassis.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.ProfileService.Infrastructure;

public sealed class ProfileContext(DbContextOptions options) : PostgresContext(options);
