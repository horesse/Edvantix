using Edvantix.Scheduling.Domain.AggregatesModel.LessonSlotAggregate;

namespace Edvantix.Scheduling.Infrastructure;

/// <summary>
/// EF Core DbContext for the Scheduling service.
/// Applies tenant isolation query filters in <see cref="OnModelCreating"/> rather than
/// in individual entity configurations, because filter expressions require access to the
/// injected <see cref="ITenantContext"/> which is not available inside
/// <c>ApplyConfigurationsFromAssembly</c>.
/// </summary>
public sealed class SchedulingDbContext(
    DbContextOptions<SchedulingDbContext> options,
    ITenantContext tenantContext
) : DbContext(options), IUnitOfWork
{
    /// <summary>Gets the lesson slots data set.</summary>
    public DbSet<LessonSlot> LessonSlots => Set<LessonSlot>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);

        // Tenant isolation query filter for LessonSlot.
        // CRITICAL: EF Core supports only one query filter per entity.
        // LessonSlot has no soft-delete, so the filter is tenant-only.
        modelBuilder
            .Entity<LessonSlot>()
            .HasQueryFilter(s => s.SchoolId == tenantContext.SchoolId);
    }

    /// <inheritdoc/>
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}
