namespace Edvantix.Blog.Domain.AggregatesModel.PostAggregate;

/// <summary>
/// Репозиторий для работы с постами блога.
/// </summary>
public interface IPostRepository : IRepository<Post>
{
    Task<IReadOnlyList<Post>> ListAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Post>> ListAsync(
        Specification<Post> spec,
        CancellationToken cancellationToken = default
    );
    Task<Post?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default);
    Task<Post?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Specification<Post> spec, CancellationToken cancellationToken = default);

    Task AddAsync(Post tag, CancellationToken cancellationToken = default);
    Task DeleteAsync(Post tag, CancellationToken cancellationToken = default);
}
