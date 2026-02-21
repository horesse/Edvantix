using Edvantix.Organizational.Features.GroupFeature.Models;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Organizational.Features.GroupFeature.Features.GetOrganizationGroups;

public class GetOrganizationGroupsEndpoint
    : IEndpoint<Ok<PagedResult<GroupModel>>, GetOrganizationGroupsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/groups",
                async (
                    [AsParameters] GetOrganizationGroupsQuery query,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(query, sender, ct)
            )
            .WithName("GetOrganizationGroups")
            .WithTags("Groups")
            .WithSummary("Группы организации")
            .WithDescription(
                "Возвращает пагинированный список групп организации. Доступно участникам."
            )
            .WithPaginationHeaders()
            .Produces<PagedResult<GroupModel>>()
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<GroupModel>>> HandleAsync(
        GetOrganizationGroupsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
