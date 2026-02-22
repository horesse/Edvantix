using Edvantix.Blog.Features.PostFeature.Models;

namespace Edvantix.Blog.Features.PostFeature.Features.GetAdminPost;

/// <summary>
/// Административный эндпоинт для получения полного содержимого поста по ID.
/// Возвращает пост любого статуса (Draft, Scheduled, Published, Archived).
/// </summary>
public sealed class GetAdminPostEndpoint : IEndpoint<Ok<PostModel>, GetAdminPostQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/admin/posts/{postId:long}",
                async (ulong postId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetAdminPostQuery(postId), sender, ct)
            )
            .WithName("GetAdminPost")
            .WithTags("Admin.Posts")
            .WithSummary("Получить пост (Admin)")
            .WithDescription(
                "Возвращает полное содержимое поста по ID для административного интерфейса. "
                    + "Работает для постов любого статуса."
            )
            .Produces<PostModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<Ok<PostModel>> HandleAsync(
        GetAdminPostQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
