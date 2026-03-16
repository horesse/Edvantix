namespace Edvantix.Blog.Features.PostFeature.Features.LikePost;

/// <summary>
/// Эндпоинт для постановки лайка на пост.
/// Требует авторизации — только зарегистрированные пользователи могут ставить лайки.
/// </summary>
public sealed class LikePostEndpoint : IEndpoint<NoContent, LikePostCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/posts/{postId:guid}/like",
                async (Guid postId, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(new LikePostCommand(postId), sender, cancellationToken)
            )
            .WithName("LikePost")
            .WithTags("Posts")
            .WithSummary("Поставить лайк")
            .WithDescription("Ставит лайк на пост. Один пользователь — один лайк.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        LikePostCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
