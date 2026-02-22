using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Blog.Features.PostFeature.Features.GetAdminPosts;

/// <summary>
/// Административный эндпоинт для получения всех постов блога, включая черновики и архивные.
/// Доступен только администраторам платформы.
/// </summary>
public sealed class GetAdminPostsEndpoint
    : IEndpoint<Ok<PagedResult<PostSummaryModel>>, GetAdminPostsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/admin/posts",
                async (
                    [AsParameters] GetAdminPostsQuery query,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(query, sender, ct)
            )
            .WithName("GetAdminPosts")
            .WithTags("Admin.Posts")
            .WithSummary("Список постов (Admin)")
            .WithDescription(
                "Возвращает пагинированный список всех постов блога любого статуса. Доступно только администраторам."
            )
            .WithPaginationHeaders()
            .Produces<PagedResult<PostSummaryModel>>()
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<Ok<PagedResult<PostSummaryModel>>> HandleAsync(
        GetAdminPostsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
