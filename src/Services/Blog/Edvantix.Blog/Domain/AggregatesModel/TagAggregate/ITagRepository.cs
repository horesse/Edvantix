using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Blog.Domain.AggregatesModel.TagAggregate;

/// <summary>
/// Репозиторий для работы с тегами блога.
/// </summary>
public interface ITagRepository : ICrudRepository<Tag, long>;
