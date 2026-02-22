using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Blog.Features.PostFeature.Features.GetPosts;

/// <summary>
/// Эндпоинт для получения пагинированного списка опубликованных постов.
/// Поддерживает фильтрацию по типу, категории, тегу и текстовый поиск.
/// </summary>
public sealed class GetPostsEndpoint
    : IEndpoint<Ok<PagedResult<PostSummaryModel>>, GetPostsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/posts",
                async ([AsParameters] GetPostsQuery query, ISender sender, CancellationToken ct) =>
                    await HandleAsync(query, sender, ct)
            )
            .WithName("GetPosts")
            .WithTags("Posts")
            .WithSummary("Список постов")
            .WithDescription(
                "Возвращает пагинированный список опубликованных постов блога с поддержкой фильтрации по типу, категории и тегам."
            )
            .WithPaginationHeaders()
            .Produces<PagedResult<PostSummaryModel>>()
            .AllowAnonymous();
    }

    public async Task<Ok<PagedResult<PostSummaryModel>>> HandleAsync(
        GetPostsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
