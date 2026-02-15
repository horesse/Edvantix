using Edvantix.Chassis.Endpoints;
using Edvantix.Company.Features.OrganizationFeature.Models;
using Edvantix.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.Company.Features.OrganizationFeature.Features.GetMyOrganizations;

public class GetMyOrganizationsEndpoint
    : IEndpoint<Ok<PagedResult<OrganizationSummaryModel>>, GetMyOrganizationsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organizations/my",
                async (
                    [AsParameters] GetMyOrganizationsQuery query,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(query, sender, ct)
            )
            .WithName("GetMyOrganizations")
            .WithTags("Organizations")
            .WithSummary("Мои организации")
            .WithDescription(
                "Возвращает пагинированный список организаций, в которых состоит текущий пользователь."
            )
            .WithPaginationHeaders()
            .Produces<PagedResult<OrganizationSummaryModel>>()
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<OrganizationSummaryModel>>> HandleAsync(
        GetMyOrganizationsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
