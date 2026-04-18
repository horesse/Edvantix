namespace Edvantix.Organizational.Features.OrganizationMembers.Get;

public sealed class GetOrganizationMemberEndpoint : IEndpoint<Ok<OrganizationMemberDto>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/members/{id:guid}",
                async (
                    Guid id,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(id, sender, cancellationToken)
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
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetOrganizationMemberQuery(id), cancellationToken);
        return TypedResults.Ok(result);
    }
}
