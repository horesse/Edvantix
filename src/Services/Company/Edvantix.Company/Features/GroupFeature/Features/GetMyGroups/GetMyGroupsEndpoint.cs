using Edvantix.Chassis.Endpoints;
using Edvantix.Company.Features.GroupFeature.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.GroupFeature.Features.GetMyGroups;

public class GetMyGroupsEndpoint
    : IEndpoint<Ok<IEnumerable<GroupSummaryModel>>, GetMyGroupsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/groups/my",
                async (ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetMyGroupsQuery(), sender, ct)
            )
            .WithName("GetMyGroups")
            .WithTags("Groups")
            .WithSummary("Мои группы")
            .WithDescription("Возвращает список групп текущего пользователя во всех организациях.")
            .Produces<IEnumerable<GroupSummaryModel>>()
            .RequireAuthorization();
    }

    public async Task<Ok<IEnumerable<GroupSummaryModel>>> HandleAsync(
        GetMyGroupsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
