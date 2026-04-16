namespace Edvantix.Organizational.Features.Organizations.Get;

public sealed class GetOrganizationEndpoint : IEndpoint<Ok<OrganizationDetailDto>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetOrganizationById")
            .WithTags("Организации")
            .WithSummary("Получить организацию")
            .Produces<OrganizationDetailDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<OrganizationDetailDto>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetOrganizationQuery(id), cancellationToken);
        return TypedResults.Ok(result);
    }
}
