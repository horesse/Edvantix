namespace Edvantix.Organizational.Features.Roles.List;

public sealed class GetRolesEndpoint : IEndpoint<Ok<PagedResult<RoleDto>>, GetRolesQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/roles",
                async (
                    [AsParameters] GetRolesQuery request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("GetRoles")
            .WithTags("Роли")
            .WithSummary("Получить список ролей организации")
            .WithPaginationHeaders()
            .Produces<PagedResult<RoleDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<RoleDto>>> HandleAsync(
        GetRolesQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);

        return TypedResults.Ok(result);
    }
}
