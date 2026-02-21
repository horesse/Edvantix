using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Blog.Domain.AggregatesModel.PostAggregate;

/// <summary>
/// Репозиторий для работы с постами блога.
/// </summary>
public interface IPostRepository : ICrudRepository<Post, ulong>;
