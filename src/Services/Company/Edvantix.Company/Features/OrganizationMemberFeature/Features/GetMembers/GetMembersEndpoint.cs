using Edvantix.Chassis.Endpoints;
using Edvantix.Company.Features.OrganizationMemberFeature.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.OrganizationMemberFeature.Features.GetMembers;

public class GetMembersEndpoint
    : IEndpoint<Ok<IEnumerable<OrganizationMemberModel>>, GetMembersQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/{orgId:long}/members",
                async (long orgId, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetMembersQuery(orgId), sender, ct)
            )
            .WithName("GetOrganizationMembers")
            .WithTags("Organization Members")
            .WithSummary("Список участников")
            .WithDescription("Возвращает список участников организации. Доступно участникам.")
            .Produces<IEnumerable<OrganizationMemberModel>>()
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Ok<IEnumerable<OrganizationMemberModel>>> HandleAsync(
        GetMembersQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
