using Edvantix.Chassis.EF.Contexts;
using Edvantix.Notification.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.Notification.Infrastructure;

public sealed class NotificationDbContext(DbContextOptions options) : PostgresContext(options)
{
    public DbSet<Outbox> Outboxes => Set<Outbox>();

    public DbSet<InAppNotification> InAppNotifications => Set<InAppNotification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);
    }
}
