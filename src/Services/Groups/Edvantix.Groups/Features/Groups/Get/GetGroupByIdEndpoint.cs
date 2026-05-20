namespace Edvantix.Groups.Features.Groups.Get;

public sealed class GetGroupByIdEndpoint : IEndpoint<Ok<GroupDetailDto>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/groups/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetGroupById")
            .WithTags("Группы")
            .WithSummary("Получить группу по идентификатору")
            .Produces<GroupDetailDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<GroupDetailDto>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetGroupByIdQuery(id), cancellationToken);
        return TypedResults.Ok(result);
    }
}
