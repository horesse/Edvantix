namespace Edvantix.Organizational.Features.Organizations.ListByProfile;

public sealed class GetMyOrganizationsEndpoint
    : IEndpoint<Ok<IReadOnlyList<OrganizationWithRoleDto>>, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/mine",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(sender, cancellationToken)
            )
            .WithName("GetMyOrganizations")
            .WithTags("Организации")
            .WithSummary("Получить мои организации")
            .WithDescription(
                "Возвращает список организаций, в которых текущий пользователь является активным участником, с его ролью в каждой из них."
            )
            .Produces<IReadOnlyList<OrganizationWithRoleDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<IReadOnlyList<OrganizationWithRoleDto>>> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetMyOrganizationsQuery(), cancellationToken);
        return TypedResults.Ok(result);
    }
}
