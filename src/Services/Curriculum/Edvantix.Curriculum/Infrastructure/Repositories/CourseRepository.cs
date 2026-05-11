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

    public async Task AddAsync(Course course, CancellationToken cancellationToken = default) =>
        await context.Courses.AddAsync(course, cancellationToken);
}
