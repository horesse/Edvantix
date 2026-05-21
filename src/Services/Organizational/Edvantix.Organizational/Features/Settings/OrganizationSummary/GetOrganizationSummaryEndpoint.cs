using Edvantix.Constants.Core;

namespace Edvantix.Organizational.Features.Settings.OrganizationSummary;

public sealed class GetOrganizationSummaryEndpoint
    : IEndpoint<Ok<OrganizationSummaryDto>, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/organization/summary",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(sender, cancellationToken)
            )
            .WithName("GetOrganizationSummary")
            .WithTags("Настройки")
            .WithSummary("Сводная информация об организации")
            .Produces<OrganizationSummaryDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<OrganizationSummaryDto>> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetOrganizationSummaryQuery(), cancellationToken);
        return TypedResults.Ok(result);
    }
}
