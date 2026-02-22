namespace Edvantix.Blog.Features.PostFeature.Features.PublishPost;

/// <summary>
/// Запрос на публикацию/планирование поста от клиента.
/// </summary>
public sealed record PublishPostRequest(DateTime? ScheduledAt = null);

/// <summary>
/// Административный эндпоинт для публикации или планирования поста.
/// </summary>
public sealed class PublishPostEndpoint : IEndpoint<NoContent, PublishPostCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/admin/posts/{postId:long}/publish",
                async (
                    Guid postId,
                    PublishPostRequest request,
                    ISender sender,
                    CancellationToken ct
                ) =>
                    await HandleAsync(
                        new PublishPostCommand(postId, request.ScheduledAt),
                        sender,
                        ct
                    )
            )
            .WithName("PublishPost")
            .WithTags("Admin.Posts")
            .WithSummary("Опубликовать пост")
            .WithDescription(
                "Публикует пост немедленно или планирует его публикацию на указанное время."
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<NoContent> HandleAsync(
        PublishPostCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
