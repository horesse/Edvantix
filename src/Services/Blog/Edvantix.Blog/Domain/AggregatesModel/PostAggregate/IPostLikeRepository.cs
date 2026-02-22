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
}
