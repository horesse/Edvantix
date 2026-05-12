using Edvantix.Chassis.Endpoints;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.Schedule.Features.GroupSchedules.Regenerate;

internal sealed class RegenerateOccurrencesEndpoint
    : IEndpoint<NoContent, RegenerateOccurrencesRequest, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/organizations/{organizationId:guid}/groups/{groupId:guid}/schedule/regenerate",
                async (
                    Guid organizationId,
                    Guid groupId,
                    [FromBody] RegenerateOccurrencesRequest request,
                    ISender sender
                ) => await HandleAsync(request with { GroupId = groupId }, sender)
            )
            .ProducesPost<object>()
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        RegenerateOccurrencesRequest request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(
            new RegenerateOccurrencesCommand(request.GroupId, request.HolidayCountryCode),
            cancellationToken
        );

        return TypedResults.NoContent();
    }
}

public sealed record RegenerateOccurrencesRequest(Guid GroupId, string? HolidayCountryCode);
