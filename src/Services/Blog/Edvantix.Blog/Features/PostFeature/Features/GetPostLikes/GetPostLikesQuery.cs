namespace Edvantix.Blog.Features.PostFeature.Features.GetPostLikes;

/// <summary>
/// Запрос для получения количества лайков поста.
/// </summary>
public sealed record GetPostLikesQuery(long PostId) : IRequest<PostLikesModel>;

/// <summary>
/// Данные о лайках поста.
/// </summary>
public sealed record PostLikesModel(long PostId, int LikesCount);

/// <summary>
/// Обработчик запроса на получение количества лайков.
/// </summary>
public sealed class GetPostLikesQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetPostLikesQuery, PostLikesModel>
{
    public async Task<PostLikesModel> Handle(
        GetPostLikesQuery request,
        CancellationToken cancellationToken
    )
    {
        using var postRepo = provider.GetRequiredService<IPostRepository>();

        var post =
            await postRepo.GetByIdAsync(request.PostId, cancellationToken)
            ?? throw new NotFoundException($"Пост с ID {request.PostId} не найден.");

        return new PostLikesModel(post.Id, post.LikesCount);
    }
}
