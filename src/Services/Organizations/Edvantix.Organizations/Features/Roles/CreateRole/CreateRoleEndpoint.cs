namespace Edvantix.Organizations.Features.Roles.CreateRole;

/// <summary>POST /v1/roles — creates a new role scoped to the current tenant.</summary>
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
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithName("CreateRole")
            .WithTags("Roles")
            .WithSummary("Create a role")
            .WithDescription("Creates a new named role scoped to the tenant from the X-School-Id header.")
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
        var location = linker.GetPathByName("GetRoleById", new { id }) ?? $"/api/v1/roles/{id}";
        return TypedResults.Created(location, id);
    }
}
