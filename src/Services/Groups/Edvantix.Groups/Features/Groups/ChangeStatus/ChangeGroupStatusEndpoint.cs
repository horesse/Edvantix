using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Groups.Features.Groups.ChangeStatus;

public sealed class ChangeGroupStatusEndpoint
    : IEndpoint<NoContent, ChangeGroupStatusCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/groups/{id:guid}/status",
                async (
                    Guid id,
                    ChangeGroupStatusRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new ChangeGroupStatusCommand(id, request.NewStatus),
                        sender,
                        cancellationToken
                    )
            )
            .WithName("ChangeGroupStatus")
            .WithTags("Группы")
            .WithSummary("Изменить статус группы")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        ChangeGroupStatusCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}

/// <summary>Тело запроса смены статуса группы.</summary>
public sealed record ChangeGroupStatusRequest(GroupStatus NewStatus);
