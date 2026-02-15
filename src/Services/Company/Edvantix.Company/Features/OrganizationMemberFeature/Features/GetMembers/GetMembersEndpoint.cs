using Edvantix.Chassis.Endpoints;
using Edvantix.Company.Features.OrganizationMemberFeature.Models;
using Edvantix.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.OrganizationMemberFeature.Features.GetMembers;

public class GetMembersEndpoint
    : IEndpoint<Ok<PagedResult<OrganizationMemberModel>>, GetMembersQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/members",
                async (
                    [AsParameters] GetMembersQuery query,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(query, sender, ct)
            )
            .WithName("GetOrganizationMembers")
            .WithTags("Organization Members")
            .WithSummary("Список участников")
            .WithDescription(
                "Возвращает пагинированный список участников организации. Доступно участникам."
            )
            .WithPaginationHeaders()
            .Produces<PagedResult<OrganizationMemberModel>>()
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<OrganizationMemberModel>>> HandleAsync(
        GetMembersQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
