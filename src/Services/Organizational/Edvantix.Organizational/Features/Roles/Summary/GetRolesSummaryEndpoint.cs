using Edvantix.Constants.Core;

namespace Edvantix.Organizational.Features.Roles.Summary;

public sealed class GetRolesSummaryEndpoint : IEndpoint<Ok<RolesSummaryDto>, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/roles/summary",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(sender, cancellationToken)
            )
            .WithName("GetRolesSummary")
            .WithTags("Роли")
            .WithSummary("Сводная информация о ролях организации")
            .Produces<RolesSummaryDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<RolesSummaryDto>> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetRolesSummaryQuery(), cancellationToken);
        return TypedResults.Ok(result);
    }
}
