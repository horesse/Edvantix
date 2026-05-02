using Edvantix.Audit.Domain.AggregatesModel.AuditEntryAggregate;
using Wolverine.EntityFrameworkCore;

namespace Edvantix.Audit.Infrastructure;

public sealed class AuditDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.MapWolverineEnvelopeStorage();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditDbContext).Assembly);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}
