namespace Edvantix.Blog.Domain.AggregatesModel.TagAggregate;

/// <summary>
/// Репозиторий для работы с тегами блога.
/// </summary>
public interface ITagRepository : IRepository<Tag>
{
    Task<IReadOnlyList<Tag>> ListAsync(CancellationToken cancellationToken = default);
    Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Tag tag, CancellationToken cancellationToken = default);
    Task DeleteAsync(Tag tag, CancellationToken cancellationToken = default);
}
