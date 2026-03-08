using Edvantix.Blog.Features.PostFeature.Models;

namespace Edvantix.Blog.Features.PostFeature.Features.GetPostBySlug;

/// <summary>
/// Запрос для получения полного содержимого поста по slug.
/// </summary>
public sealed record GetPostBySlugQuery(string Slug) : IQuery<PostModel>;

/// <summary>
/// Обработчик запроса на получение поста по slug.
/// Для премиум-постов возвращает данные независимо от статуса подписки на уровне API
/// (проверка осуществляется клиентом или через middleware в будущем).
/// </summary>
public sealed class GetPostBySlugQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetPostBySlugQuery, PostModel>
{
    public async ValueTask<PostModel> Handle(
        GetPostBySlugQuery request,
        CancellationToken cancellationToken
    )
    {
        var postRepo = provider.GetRequiredService<IPostRepository>();

        var post =
            await postRepo.GetBySlugAsync(request.Slug, cancellationToken)
            ?? throw new NotFoundException($"Пост со slug '{request.Slug}' не найден.");

        if (post.Status != PostStatus.Published)
            throw new NotFoundException($"Пост со slug '{request.Slug}' не найден.");

        var mapper = provider.GetRequiredService<IMapper<Post, PostModel>>();

        var result = mapper.Map(post);
        await result.EnrichIsLikeByMe(provider, cancellationToken);
        await result.EnrichAuthor(post.AuthorId, provider, cancellationToken);

        return result;
    }
}
