using Edvantix.Chassis.Endpoints;

namespace Edvantix.Schedule.Features.GroupSchedules.Get;

internal sealed class GetGroupScheduleEndpoint : IEndpoint<Ok<GroupScheduleDto>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/{organizationId:guid}/groups/{groupId:guid}/schedule",
                async (Guid organizationId, Guid groupId, ISender sender) =>
                    await HandleAsync(groupId, sender)
            )
            .ProducesGet<GroupScheduleDto>()
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<GroupScheduleDto>> HandleAsync(
        Guid groupId,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetGroupScheduleQuery(groupId), cancellationToken);
        return TypedResults.Ok(result);
    }
}
