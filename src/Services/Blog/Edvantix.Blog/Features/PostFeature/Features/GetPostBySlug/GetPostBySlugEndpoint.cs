using Edvantix.Blog.Features.PostFeature.Models;

namespace Edvantix.Blog.Features.PostFeature.Features.GetPostBySlug;

public sealed class GetPostBySlugEndpoint : IEndpoint<Ok<PostModel>, GetPostBySlugQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/posts/{slug}",
                async (string slug, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(new GetPostBySlugQuery(slug), sender, cancellationToken)
            )
            .WithName("Получить пост")
            .WithTags("Посты")
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
