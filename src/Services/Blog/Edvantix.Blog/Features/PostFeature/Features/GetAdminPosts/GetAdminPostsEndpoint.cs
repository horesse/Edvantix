using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Blog.Features.PostFeature.Features.GetAdminPosts;

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
                    CancellationToken cancellationToken
                ) => await HandleAsync(query, sender, cancellationToken)
            )
            .WithName("Список постов (Admin)")
            .WithTags("Администрирование")
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
