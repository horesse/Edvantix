namespace Edvantix.Groups.Features.Groups.Update;

public sealed class UpdateGroupEndpoint : IEndpoint<NoContent, UpdateGroupCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/groups/{id:guid}",
                async (
                    Guid id,
                    UpdateGroupCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command with { Id = id }, sender, cancellationToken)
            )
            .WithName("UpdateGroup")
            .WithTags("Группы")
            .WithSummary("Обновить учебную группу")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateGroupCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
