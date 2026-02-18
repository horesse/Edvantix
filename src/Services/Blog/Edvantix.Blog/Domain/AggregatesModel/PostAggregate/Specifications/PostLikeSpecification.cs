using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Builders;

namespace Edvantix.Blog.Domain.AggregatesModel.PostAggregate.Specifications;

/// <summary>
/// Спецификация для поиска лайков по идентификатору поста и/или пользователя.
/// PostLike не является агрегатным корнем, поэтому наследуется напрямую от Specification.
/// Все условия применяются единожды в конструкторе для корректной генерации SQL.
/// </summary>
public sealed class PostLikeSpecification : Specification<PostLike>
{
    /// <summary>
    /// Создаёт спецификацию с заданными критериями фильтрации.
    /// </summary>
    /// <param name="postId">Фильтр по идентификатору поста.</param>
    /// <param name="userId">Фильтр по идентификатору пользователя.</param>
    public PostLikeSpecification(long? postId = null, long? userId = null)
    {
        Query.Where(l =>
            (postId == null || l.PostId == postId) && (userId == null || l.UserId == userId)
        );
    }
}
