using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;
using Edvantix.Schedule.Domain.AggregatesModel.HolidayAggregate;
using Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;
using Wolverine.EntityFrameworkCore;

namespace Edvantix.Schedule.Infrastructure;

public sealed class ScheduleDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    public DbSet<GroupSchedule> GroupSchedules => Set<GroupSchedule>();
    public DbSet<ScheduleSlot> ScheduleSlots => Set<ScheduleSlot>();
    public DbSet<ScheduleException> ScheduleExceptions => Set<ScheduleException>();
    public DbSet<LessonOccurrence> LessonOccurrences => Set<LessonOccurrence>();
    public DbSet<Holiday> Holidays => Set<Holiday>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.MapWolverineEnvelopeStorage();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ScheduleDbContext).Assembly);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}
