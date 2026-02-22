namespace Edvantix.Blog.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория тегов блога на основе BlogDbContext.
/// </summary>
public sealed class TagRepository(BlogDbContext dbContext) : ITagRepository
{
    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;

    public async Task<IReadOnlyList<Tag>> ListAsync(CancellationToken cancellationToken = default)
    {
        var tags = dbContext.Set<Tag>();
        return await tags.OrderBy(t => t.Name).ToListAsync(cancellationToken);
    }

    public async Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tags = dbContext.Set<Tag>();
        return await tags.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task AddAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        var tags = dbContext.Set<Tag>();
        await tags.AddAsync(tag, cancellationToken);
    }

    public Task DeleteAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        var tags = dbContext.Set<Tag>();
        tags.Remove(tag);
        return Task.CompletedTask;
    }
}
