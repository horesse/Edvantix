using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Blog.Infrastructure.Repositories;

public sealed class PostLikeRepository(BlogDbContext dbContext) : IPostLikeRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;
}
