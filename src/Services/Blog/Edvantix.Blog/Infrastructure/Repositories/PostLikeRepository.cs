using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Blog.Infrastructure.Repositories;

public sealed class PostLikeRepository(BlogDbContext dbContext) : IPostLikeRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;

    public async Task<PostLike?> Get(
        Specification<PostLike> spec,
        CancellationToken cancellationToken = default
    )
    {
        var likes = dbContext.Set<PostLike>();
        return await Spec.GetQuery(likes, spec).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(PostLike like, CancellationToken cancellationToken = default)
    {
        var likes = dbContext.Set<PostLike>();
        await likes.AddAsync(like, cancellationToken);
    }

    public Task DeleteAsync(PostLike like, CancellationToken cancellationToken = default)
    {
        var likes = dbContext.Set<PostLike>();
        likes.Remove(like);
        return Task.CompletedTask;
    }
}
