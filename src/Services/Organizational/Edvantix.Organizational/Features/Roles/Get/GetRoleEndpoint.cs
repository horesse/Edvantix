namespace Edvantix.Organizational.Features.Roles.Get;

public sealed class GetRoleEndpoint : IEndpoint<Ok<RoleDetailDto>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/roles/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetRoleById")
            .WithTags("Роли")
            .WithSummary("Получить роль организации")
            .Produces<RoleDetailDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<RoleDetailDto>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetRoleQuery(id), cancellationToken);

        return TypedResults.Ok(result);
    }
}
