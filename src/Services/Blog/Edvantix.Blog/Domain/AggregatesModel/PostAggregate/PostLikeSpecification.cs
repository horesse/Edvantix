namespace Edvantix.Blog.Domain.AggregatesModel.PostAggregate;

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
    public PostLikeSpecification(Guid? postId = null, Guid? userId = null)
    {
        Query.Where(l =>
            (postId == null || l.PostId == postId) && (userId == null || l.ProfileId == userId)
        );
    }
}
