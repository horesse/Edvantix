namespace Edvantix.Blog.Features.PostFeature.Features.GetPostLikes;

/// <summary>
/// Эндпоинт для получения количества лайков поста.
/// </summary>
public sealed class GetPostLikesEndpoint : IEndpoint<Ok<PostLikesModel>, GetPostLikesQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/posts/{postId:guid}/likes",
                async (Guid postId, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(new GetPostLikesQuery(postId), sender, cancellationToken)
            )
            .WithName("GetPostLikes")
            .WithTags("Posts")
            .WithSummary("Количество лайков")
            .WithDescription("Возвращает количество лайков у указанного поста.")
            .Produces<PostLikesModel>()
            .Produces(StatusCodes.Status404NotFound)
            .AllowAnonymous();
    }

    public async Task<Ok<PostLikesModel>> HandleAsync(
        GetPostLikesQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
