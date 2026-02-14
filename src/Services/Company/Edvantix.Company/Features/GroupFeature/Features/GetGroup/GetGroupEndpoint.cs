using Edvantix.Chassis.Endpoints;
using Edvantix.Company.Features.GroupFeature.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Company.Features.GroupFeature.Features.GetGroup;

public class GetGroupEndpoint : IEndpoint<Ok<GroupModel>, GetGroupQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/groups/{id:long}",
                async (long id, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetGroupQuery(id), sender, ct)
            )
            .WithName("GetGroup")
            .WithTags("Groups")
            .WithSummary("Получить группу")
            .WithDescription("Возвращает детальную информацию о группе. Доступно участникам организации.")
            .Produces<GroupModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Ok<GroupModel>> HandleAsync(
        GetGroupQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
