using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Blog.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория тегов блога на основе BlogDbContext.
/// </summary>
public sealed class TagRepository(BlogDbContext dbContext) : ITagRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;
}
