using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Blog.Features.PostFeature.Features.GetPosts;

public sealed class GetPostsEndpoint
    : IEndpoint<Ok<PagedResult<PostSummaryModel>>, GetPostsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/posts",
                async (
                    [AsParameters] GetPostsQuery query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(query, sender, cancellationToken)
            )
            .WithName("Список постов")
            .WithTags("Посты")
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
