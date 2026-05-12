using Edvantix.Chassis.Endpoints;

namespace Edvantix.Schedule.Features.LessonOccurrences.Get;

internal sealed class GetLessonOccurrencesEndpoint
    : IEndpoint<Ok<IReadOnlyList<LessonOccurrenceDto>>, (Guid GroupId, DateOnly From, DateOnly To), ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/{organizationId:guid}/groups/{groupId:guid}/schedule/occurrences",
                async (
                    Guid organizationId,
                    Guid groupId,
                    DateOnly from,
                    DateOnly to,
                    ISender sender
                ) =>
                    await HandleAsync((groupId, from, to), sender)
            )
            .ProducesGet<IReadOnlyList<LessonOccurrenceDto>>()
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<IReadOnlyList<LessonOccurrenceDto>>> HandleAsync(
        (Guid GroupId, DateOnly From, DateOnly To) request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(
            new GetLessonOccurrencesQuery(request.GroupId, request.From, request.To),
            cancellationToken
        );

        return TypedResults.Ok(result);
    }
}
