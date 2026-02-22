using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Blog.Infrastructure.Repositories;

public sealed class CategoryRepository(BlogDbContext dbContext) : ICategoryRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;
}
