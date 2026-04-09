namespace Edvantix.Identity.Infrastructure;

/// <summary>
/// Контекст БД сервиса Identity.
/// Содержит только таблицы MassTransit outbox для гарантированной доставки событий.
/// </summary>
public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
