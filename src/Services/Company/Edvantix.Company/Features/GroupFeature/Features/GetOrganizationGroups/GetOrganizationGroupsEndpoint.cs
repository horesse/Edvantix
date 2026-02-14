using Edvantix.Chassis.Endpoints;
using Edvantix.Company.Features.GroupFeature.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.GroupFeature.Features.GetOrganizationGroups;

public class GetOrganizationGroupsEndpoint
    : IEndpoint<Ok<IEnumerable<GroupModel>>, GetOrganizationGroupsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/{orgId:long}/groups",
                async (long orgId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetOrganizationGroupsQuery(orgId), sender, ct)
            )
            .WithName("GetOrganizationGroups")
            .WithTags("Groups")
            .WithSummary("Группы организации")
            .WithDescription("Возвращает список групп организации. Доступно участникам.")
            .Produces<IEnumerable<GroupModel>>()
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Ok<IEnumerable<GroupModel>>> HandleAsync(
        GetOrganizationGroupsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
