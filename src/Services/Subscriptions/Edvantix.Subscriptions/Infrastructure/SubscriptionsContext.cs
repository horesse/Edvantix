using Edvantix.Chassis.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.Subscriptions.Infrastructure;

/// <summary>
/// Database context for the Subscriptions microservice.
/// </summary>
/// <param name="options">The database context options.</param>
public sealed class SubscriptionsContext(DbContextOptions options) : PostgresContext(options);
