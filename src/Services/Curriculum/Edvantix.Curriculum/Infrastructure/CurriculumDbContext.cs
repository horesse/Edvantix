using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;
using Wolverine.EntityFrameworkCore;

namespace Edvantix.Curriculum.Infrastructure;

public sealed class CurriculumDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.MapWolverineEnvelopeStorage();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CurriculumDbContext).Assembly);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}
