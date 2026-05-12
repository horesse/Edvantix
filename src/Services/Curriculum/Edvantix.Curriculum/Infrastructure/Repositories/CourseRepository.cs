using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

namespace Edvantix.Curriculum.Infrastructure.Repositories;

internal sealed class CourseRepository(CurriculumDbContext context) : ICourseRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task<Course?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Courses.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Course?> GetByIdForWriteAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Courses.Include(c => c.Goals)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Course?> GetByIdWithModulesAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Courses.AsNoTracking()
            .Include(c => c.Goals)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Course?> GetByModuleIdAsync(
        Guid moduleId,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Courses.Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .FirstOrDefaultAsync(c => c.Modules.Any(m => m.Id == moduleId), cancellationToken);

    public async Task<Course?> GetByLessonIdAsync(
        Guid lessonId,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Courses.Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .FirstOrDefaultAsync(
                c => c.Modules.Any(m => m.Lessons.Any(l => l.Id == lessonId)),
                cancellationToken
            );

    public async Task AddAsync(Course course, CancellationToken cancellationToken = default) =>
        await context.Courses.AddAsync(course, cancellationToken);

    public async Task<IReadOnlyList<Course>> ListAsync(
        Specification<Course> specification,
        CancellationToken cancellationToken = default
    )
    {
        var query = Specification.GetQuery(context.Courses.AsQueryable(), specification);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Specification<Course> specification,
        CancellationToken cancellationToken = default
    )
    {
        var query = Specification.GetQuery(context.Courses.AsQueryable(), specification);
        return await query.CountAsync(cancellationToken);
    }
}
