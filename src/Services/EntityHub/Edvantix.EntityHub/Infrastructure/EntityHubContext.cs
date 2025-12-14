using Edvantix.Chassis.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.EntityHub.Infrastructure;

public class EntityHubContext(DbContextOptions<EntityHubContext> options)
    : PostgresContext(options);
