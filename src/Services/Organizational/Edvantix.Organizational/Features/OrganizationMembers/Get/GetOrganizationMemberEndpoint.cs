namespace Edvantix.Organizational.Features.OrganizationMembers.Get;

public sealed class GetOrganizationMemberEndpoint
    : IEndpoint<Ok<OrganizationMemberDto>, Guid, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/{organizationId:guid}/members/{id:guid}",
                async (
                    Guid organizationId,
                    Guid id,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(organizationId, id, sender, cancellationToken)
            )
            .WithName("GetOrganizationMemberById")
            .WithTags("Участники организации")
            .WithSummary("Получить участника организации")
            .Produces<OrganizationMemberDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<OrganizationMemberDto>> HandleAsync(
        Guid organizationId,
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(
            new GetOrganizationMemberQuery(organizationId, id),
            cancellationToken
        );
        return TypedResults.Ok(result);
    }
}
