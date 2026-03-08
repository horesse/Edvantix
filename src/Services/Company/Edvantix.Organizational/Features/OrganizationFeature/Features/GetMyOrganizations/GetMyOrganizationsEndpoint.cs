using Edvantix.Organizational.Features.OrganizationFeature.Models;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.GetMyOrganizations;

public sealed class GetMyOrganizationsEndpoint
    : IEndpoint<Ok<IEnumerable<OrganizationSummaryModel>>, GetMyOrganizationsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/my",
                async (ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetMyOrganizationsQuery(), sender, ct)
            )
            .WithName("GetMyOrganizations")
            .WithTags("Organizations")
            .WithSummary("Мои организации")
            .WithDescription(
                "Возвращает список организаций, в которых состоит текущий пользователь."
            )
            .Produces<IEnumerable<OrganizationSummaryModel>>()
            .RequireAuthorization();
    }

    public async Task<Ok<IEnumerable<OrganizationSummaryModel>>> HandleAsync(
        GetMyOrganizationsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
