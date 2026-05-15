namespace Edvantix.Organizational.Features.Levels.Reorder;

public sealed class ReorderLevelsEndpoint : IEndpoint<NoContent, ReorderLevelsCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Маршрут /levels/reorder регистрируется до /levels/{id:guid}, чтобы не конфликтовать.
        app.MapPut(
                "/levels/reorder",
                async (
                    ReorderLevelsCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("ReorderLevels")
            .WithTags("Уровни")
            .WithSummary("Переупорядочить уровни справочника")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        ReorderLevelsCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
