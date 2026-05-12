using Edvantix.Chassis.Endpoints;

namespace Edvantix.Schedule.Features.GroupSchedules.RemoveException;

internal sealed class RemoveScheduleExceptionEndpoint
    : IEndpoint<NoContent, (Guid GroupId, Guid ExceptionId), ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/organizations/{organizationId:guid}/groups/{groupId:guid}/schedule/exceptions/{exceptionId:guid}",
                async (Guid organizationId, Guid groupId, Guid exceptionId, ISender sender) =>
                    await HandleAsync((groupId, exceptionId), sender)
            )
            .ProducesDelete()
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        (Guid GroupId, Guid ExceptionId) request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(
            new RemoveScheduleExceptionCommand(request.GroupId, request.ExceptionId),
            cancellationToken
        );

        return TypedResults.NoContent();
    }
}
