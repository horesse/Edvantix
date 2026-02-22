namespace Edvantix.Blog.Domain.AggregatesModel.PostAggregate;

/// <summary>
/// Репозиторий для работы с лайками постов.
/// </summary>
public interface IPostLikeRepository : IRepository<PostLike>
{
    Task<PostLike?> Get(
        Specification<PostLike> spec,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(PostLike like, CancellationToken cancellationToken = default);
    Task DeleteAsync(PostLike like, CancellationToken cancellationToken = default);
}
