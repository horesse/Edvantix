namespace Edvantix.Blog.Domain.AggregatesModel.CategoryAggregate;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IReadOnlyList<Category>> ListAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default);

    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task DeleteAsync(Category category, CancellationToken cancellationToken = default);
}
