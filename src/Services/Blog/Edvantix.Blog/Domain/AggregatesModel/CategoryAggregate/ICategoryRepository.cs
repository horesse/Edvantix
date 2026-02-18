using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Blog.Domain.AggregatesModel.CategoryAggregate;

/// <summary>
/// Репозиторий для работы с категориями блога.
/// </summary>
public interface ICategoryRepository : ICrudRepository<Category, long>;
