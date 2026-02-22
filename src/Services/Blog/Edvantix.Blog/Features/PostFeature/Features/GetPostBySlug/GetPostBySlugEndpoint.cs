using Edvantix.Blog.Features.PostFeature.Models;

namespace Edvantix.Blog.Features.PostFeature.Features.GetPostBySlug;

/// <summary>
/// Эндпоинт для получения полного содержимого поста по slug.
/// </summary>
public sealed class GetPostBySlugEndpoint : IEndpoint<Ok<PostModel>, GetPostBySlugQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/posts/{slug}",
                async (string slug, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetPostBySlugQuery(slug), sender, ct)
            )
            .WithName("GetPostBySlug")
            .WithTags("Posts")
            .WithSummary("Получить пост")
            .WithDescription("Возвращает полное содержимое опубликованного поста по его slug.")
            .Produces<PostModel>()
            .Produces(StatusCodes.Status404NotFound)
            .AllowAnonymous();
    }

    public async Task<Ok<PostModel>> HandleAsync(
        GetPostBySlugQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
