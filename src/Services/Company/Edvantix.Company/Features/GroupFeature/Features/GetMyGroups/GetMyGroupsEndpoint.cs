using Edvantix.Chassis.Endpoints;
using Edvantix.Company.Features.GroupFeature.Models;
using Edvantix.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.Company.Features.GroupFeature.Features.GetMyGroups;

public class GetMyGroupsEndpoint
    : IEndpoint<Ok<PagedResult<GroupSummaryModel>>, GetMyGroupsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/groups/my",
                async (
                    [AsParameters] GetMyGroupsQuery query,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(query, sender, ct)
            )
            .WithName("GetMyGroups")
            .WithTags("Groups")
            .WithSummary("Мои группы")
            .WithDescription(
                "Возвращает пагинированный список групп текущего пользователя во всех организациях."
            )
            .WithPaginationHeaders()
            .Produces<PagedResult<GroupSummaryModel>>()
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<GroupSummaryModel>>> HandleAsync(
        GetMyGroupsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
