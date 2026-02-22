using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Blog.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория постов блога на основе BlogDbContext.
/// </summary>
public sealed class PostRepository(BlogDbContext dbContext) : IPostRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;
}
