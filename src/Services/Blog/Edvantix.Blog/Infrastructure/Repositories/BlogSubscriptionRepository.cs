using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Blog.Infrastructure.Repositories;

internal sealed class BlogSubscriptionRepository(BlogDbContext dbContext)
    : IBlogSubscriptionRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;
}
