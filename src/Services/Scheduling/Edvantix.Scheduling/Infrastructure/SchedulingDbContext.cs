namespace Edvantix.Scheduling.Infrastructure;

/// <summary>
/// EF Core DbContext for the Scheduling service.
/// Applies tenant isolation query filters in <see cref="OnModelCreating"/> rather than
/// in individual entity configurations, because filter expressions require access to the
/// injected <see cref="ITenantContext"/> which is not available inside
/// <c>ApplyConfigurationsFromAssembly</c>.
/// Domain entity DbSets and HasQueryFilter calls will be added in Plan 02 when LessonSlot is created.
/// </summary>
public sealed class SchedulingDbContext(
    DbContextOptions<SchedulingDbContext> options,
    ITenantContext tenantContext
) : DbContext(options), IUnitOfWork
{
    // DbSets for domain entities (LessonSlot etc.) will be added in Plan 02.

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);

        // Tenant isolation query filters for domain entities will be added in Plan 02.
        // CRITICAL: EF Core supports only one HasQueryFilter per entity.
        _ = tenantContext; // Referenced here to satisfy primary constructor — filters added in Plan 02.
    }

    /// <inheritdoc/>
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}
