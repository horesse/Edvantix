namespace Edvantix.Blog.Infrastructure.Repositories;

public sealed class CategoryRepository(BlogDbContext dbContext) : ICategoryRepository
{
    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;

    public async Task<IReadOnlyList<Category>> ListAsync(
        CancellationToken cancellationToken = default
    )
    {
        var categories = dbContext.Set<Category>();
        return await categories.OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(
        ulong id,
        CancellationToken cancellationToken = default
    )
    {
        var categories = dbContext.Set<Category>();
        return await categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        var categories = dbContext.Set<Category>();
        await categories.AddAsync(category, cancellationToken);
    }

    public Task<Category> UpdateAsync(
        Category category,
        CancellationToken cancellationToken = default
    )
    {
        var categories = dbContext.Set<Category>();
        return Task.FromResult(categories.Update(category).Entity);
    }

    public Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
    {
        var categories = dbContext.Set<Category>();
        categories.Remove(category);
        return Task.CompletedTask;
    }
}
