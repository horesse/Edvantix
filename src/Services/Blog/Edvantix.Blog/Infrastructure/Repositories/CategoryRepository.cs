using Edvantix.Blog.Domain.AggregatesModel.CategoryAggregate;
using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Blog.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория категорий блога на основе BlogContext.
/// </summary>
public sealed class CategoryRepository(IServiceProvider provider)
    : CrudRepository<BlogContext, Category, ulong>(provider),
        ICategoryRepository;
