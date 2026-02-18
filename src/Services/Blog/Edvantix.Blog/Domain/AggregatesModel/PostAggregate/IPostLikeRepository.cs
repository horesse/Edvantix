using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Blog.Domain.AggregatesModel.PostAggregate;

/// <summary>
/// Репозиторий для работы с лайками постов.
/// </summary>
public interface IPostLikeRepository : ICrudRepository<PostLike, long>;
