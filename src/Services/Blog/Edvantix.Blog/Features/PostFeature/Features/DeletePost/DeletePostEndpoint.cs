namespace Edvantix.Blog.Features.PostFeature.Features.DeletePost;

/// <summary>
/// Административный эндпоинт для архивирования поста.
/// Пост переводится в статус Archived и скрывается из публичного списка.
/// </summary>
public sealed class DeletePostEndpoint : IEndpoint<NoContent, DeletePostCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/admin/posts/{postId:long}",
                async (Guid postId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new DeletePostCommand(postId), sender, ct)
            )
            .WithName("DeletePost")
            .WithTags("Admin.Posts")
            .WithSummary("Архивировать пост")
            .WithDescription(
                "Переводит пост в архив. Пост скрывается из публичного списка, но данные сохраняются."
            )
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<NoContent> HandleAsync(
        DeletePostCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
