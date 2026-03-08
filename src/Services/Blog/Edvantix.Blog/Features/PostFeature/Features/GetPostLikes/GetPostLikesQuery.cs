namespace Edvantix.Blog.Features.PostFeature.Features.GetPostLikes;

using Mediator;

/// <summary>
/// Запрос для получения количества лайков поста.
/// </summary>
public sealed record GetPostLikesQuery(Guid PostId) : IQuery<PostLikesModel>;

/// <summary>
/// Данные о лайках поста.
/// </summary>
public sealed record PostLikesModel(Guid PostId, int LikesCount);

/// <summary>
/// Обработчик запроса на получение количества лайков.
/// </summary>
public sealed class GetPostLikesQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetPostLikesQuery, PostLikesModel>
{
    public async ValueTask<PostLikesModel> Handle(
        GetPostLikesQuery request,
        CancellationToken cancellationToken
    )
    {
        var postRepo = provider.GetRequiredService<IPostRepository>();

        var post =
            await postRepo.GetByIdAsync(request.PostId, cancellationToken)
            ?? throw new NotFoundException($"Пост с ID {request.PostId} не найден.");

        return new PostLikesModel(post.Id, post.LikesCount);
    }
}
