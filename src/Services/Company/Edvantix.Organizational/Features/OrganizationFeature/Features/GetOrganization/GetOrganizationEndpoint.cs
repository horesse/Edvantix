using Edvantix.Organizational.Features.OrganizationFeature.Models;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.GetOrganization;

public class GetOrganizationEndpoint
    : IEndpoint<Ok<OrganizationModel>, GetOrganizationQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/{id:long}",
                async (long id, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetOrganizationQuery((ulong)id), sender, ct)
            )
            .WithName("GetOrganization")
            .WithTags("Organizations")
            .WithSummary("Получить организацию")
            .WithDescription(
                "Возвращает детальную информацию об организации. Доступно участникам организации."
            )
            .Produces<OrganizationModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization();
    }

    public async Task<Ok<OrganizationModel>> HandleAsync(
        GetOrganizationQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
