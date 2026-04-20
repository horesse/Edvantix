namespace Edvantix.Organizational.Features.Roles.Create;

public sealed class CreateRoleEndpoint
    : IEndpoint<Created<Guid>, CreateRoleCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/roles",
                async (
                    CreateRoleCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, linker, cancellationToken)
            )
            .WithName("CreateRole")
            .WithTags("Роли")
            .WithSummary("Создать роль в организации")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateRoleCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        var location = linker.GetPathByName("GetRoleById", new { id }) ?? $"/api/roles/{id}";

        return TypedResults.Created(location, id);
    }
}
