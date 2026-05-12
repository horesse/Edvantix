using Edvantix.Chassis.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.Schedule.Features.GroupSchedules.AddException;

internal sealed class AddScheduleExceptionEndpoint
    : IEndpoint<Created<Guid>, AddScheduleExceptionRequest, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations/{organizationId:guid}/groups/{groupId:guid}/schedule/exceptions",
                async (
                    Guid organizationId,
                    Guid groupId,
                    [FromBody] AddScheduleExceptionRequest request,
                    ISender sender
                ) =>
                    await HandleAsync(request with { GroupId = groupId }, sender)
            )
            .ProducesPost<Guid>()
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        AddScheduleExceptionRequest request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(
            new AddScheduleExceptionCommand(request.GroupId, request.ExceptionDate, request.Reason),
            cancellationToken
        );

        return TypedResults.Created(
            $"/api/v1/groups/{request.GroupId}/schedule/exceptions/{id}",
            id
        );
    }
}

public sealed record AddScheduleExceptionRequest(
    Guid GroupId,
    DateOnly ExceptionDate,
    string? Reason
);
