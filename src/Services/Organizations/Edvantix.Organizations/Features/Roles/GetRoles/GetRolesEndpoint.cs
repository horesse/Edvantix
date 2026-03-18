namespace Edvantix.Organizations.Features.Roles.GetRoles;

/// <summary>GET /v1/roles — returns all roles for the current tenant.</summary>
public sealed class GetRolesEndpoint : IEndpoint<Ok<List<RoleListItem>>, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/roles",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(sender, cancellationToken)
            )
            .Produces<List<RoleListItem>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName("GetRoles")
            .WithTags("Roles")
            .WithSummary("List roles")
            .WithDescription("Returns all roles for the current tenant. Does not include permissions.")
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<List<RoleListItem>>> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetRolesQuery(), cancellationToken);
        return TypedResults.Ok(result);
    }
}
