using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;
using Wolverine.EntityFrameworkCore;

namespace Edvantix.Groups.Infrastructure;

/// <summary>
/// Контекст базы данных для сервиса групп.
/// </summary>
public sealed class GroupsDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    /// <summary>Справочник уровней сложности организации.</summary>
    public DbSet<Level> Levels { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.MapWolverineEnvelopeStorage();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GroupsDbContext).Assembly);
    }

    /// <inheritdoc />
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}
