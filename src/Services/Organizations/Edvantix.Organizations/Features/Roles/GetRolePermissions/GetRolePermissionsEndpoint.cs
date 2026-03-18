namespace Edvantix.Organizations.Features.Roles.GetRolePermissions;

/// <summary>GET /v1/roles/{id}/permissions — returns permissions assigned to a role.</summary>
public sealed class GetRolePermissionsEndpoint
    : IEndpoint<Ok<List<RolePermissionItem>>, GetRolePermissionsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/roles/{id:guid}/permissions",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(
                        new GetRolePermissionsQuery { RoleId = id },
                        sender,
                        cancellationToken
                    )
            )
            .Produces<List<RolePermissionItem>>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName("GetRolePermissions")
            .WithTags("Roles")
            .WithSummary("Get role permissions")
            .WithDescription(
                "Returns all permissions currently assigned to the role, ordered by name."
            )
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<List<RolePermissionItem>>> HandleAsync(
        GetRolePermissionsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
