namespace Edvantix.Organizational.Features.Organizations.List;

public sealed class GetOrganizationsEndpoint
    : IEndpoint<Ok<PagedResult<OrganizationDto>>, GetOrganizationsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations",
                async (
                    [AsParameters] GetOrganizationsQuery request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("GetOrganizations")
            .WithTags("Организации")
            .WithSummary("Получить список организаций")
            .WithDescription("Возвращает постраничный список организаций с поиском и фильтрацией")
            .WithPaginationHeaders()
            .Produces<PagedResult<OrganizationDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1);
    }

    public async Task<Ok<PagedResult<OrganizationDto>>> HandleAsync(
        GetOrganizationsQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return TypedResults.Ok(result);
    }
}
