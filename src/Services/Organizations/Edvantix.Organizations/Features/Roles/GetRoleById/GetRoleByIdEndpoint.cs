namespace Edvantix.Organizations.Features.Roles.GetRoleById;

/// <summary>GET /v1/roles/{id} — returns a single role by identifier.</summary>
public sealed class GetRoleByIdEndpoint : IEndpoint<Ok<RoleDetailItem>, GetRoleByIdQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/roles/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(new GetRoleByIdQuery { Id = id }, sender, cancellationToken)
            )
            .Produces<RoleDetailItem>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName("GetRoleById")
            .WithTags("Roles")
            .WithSummary("Get role by ID")
            .WithDescription("Returns the name and identifier of a single role.")
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<RoleDetailItem>> HandleAsync(
        GetRoleByIdQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
