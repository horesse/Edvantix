namespace Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

/// <summary>Репозиторий агрегата <see cref="Course"/>.</summary>
public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Course course, CancellationToken cancellationToken = default);
}
