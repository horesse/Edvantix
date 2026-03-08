using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Blog.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория постов блога на основе BlogDbContext.
/// </summary>
internal sealed class PostRepository(BlogDbContext dbContext) : IPostRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;

    public async Task<IReadOnlyList<Post>> ListAsync(CancellationToken cancellationToken = default)
    {
        var posts = dbContext.Set<Post>();
        var spec = new PostSpecification(includeRelations: true);
        var query = Spec.GetQuery(posts, spec);

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Post>> ListAsync(
        Specification<Post> spec,
        CancellationToken cancellationToken = default
    )
    {
        var posts = dbContext.Set<Post>();
        return await Spec.GetQuery(posts, spec).ToListAsync(cancellationToken);
    }

    public async Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var posts = dbContext.Set<Post>();
        var spec = new PostByIdSpecification(id);

        return await Spec.GetQuery(posts, spec).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Post?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default
    )
    {
        var posts = dbContext.Set<Post>();
        var spec = new PostBySlugSpecification(slug);

        return await Spec.GetQuery(posts, spec).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Specification<Post> spec,
        CancellationToken cancellationToken = default
    )
    {
        var posts = dbContext.Set<Post>();
        return await Spec.GetQuery(posts, spec).CountAsync(cancellationToken);
    }

    public async Task AddAsync(Post tag, CancellationToken cancellationToken = default)
    {
        var posts = dbContext.Set<Post>();
        await posts.AddAsync(tag, cancellationToken);
    }

    public Task DeleteAsync(Post tag, CancellationToken cancellationToken = default)
    {
        var posts = dbContext.Set<Post>();
        posts.Remove(tag);
        return Task.CompletedTask;
    }
}
