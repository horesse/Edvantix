using Edvantix.Blog.Domain.AggregatesModel.TagAggregate;
using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Blog.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория тегов блога на основе BlogContext.
/// </summary>
public sealed class TagRepository(IServiceProvider provider)
    : CrudRepository<BlogContext, Tag, long>(provider),
        ITagRepository;
