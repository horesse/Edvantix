using Edvantix.Blog.Features.PostFeature.Models;

namespace Edvantix.Blog.Features.PostFeature.Features.GetAdminPost;

/// <summary>
/// Административный запрос для получения полного содержимого поста по ID.
/// В отличие от публичного GetPostBySlugQuery возвращает пост любого статуса.
/// </summary>
public sealed record GetAdminPostQuery(ulong PostId) : IRequest<PostModel>;

/// <summary>
/// Обработчик административного запроса на получение поста.
/// </summary>
public sealed class GetAdminPostQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetAdminPostQuery, PostModel>
{
    public async ValueTask<PostModel> Handle(
        GetAdminPostQuery request,
        CancellationToken cancellationToken
    )
    {
        var postRepo = provider.GetRequiredService<IPostRepository>();

        var post =
            await postRepo.GetByIdAsync(request.PostId, cancellationToken)
            ?? throw new NotFoundException($"Пост с ID {request.PostId} не найден.");

        var mapper = provider.GetRequiredService<IMapper<Post, PostModel>>();

        var result = mapper.Map(post);
        await result.EnrichIsLikeByMe(provider, cancellationToken);
        await result.EnrichAuthor(post.AuthorId, provider, cancellationToken);

        return result;
    }
}
