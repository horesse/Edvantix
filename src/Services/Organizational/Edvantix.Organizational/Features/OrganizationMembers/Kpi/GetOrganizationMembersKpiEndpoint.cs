namespace Edvantix.Organizational.Features.OrganizationMembers.Kpi;

public sealed class GetOrganizationMembersKpiEndpoint
    : IEndpoint<Ok<OrganizationMembersKpiDto>, GetOrganizationMembersKpiQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/members/kpi",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(
                        new GetOrganizationMembersKpiQuery(),
                        sender,
                        cancellationToken
                    )
            )
            .WithName("GetOrganizationMembersKpi")
            .WithTags("Участники организации")
            .WithSummary("Получить KPI-статистику участников организации")
            .WithDescription(
                "Возвращает количество участников в разрезе статусов: всего, активных, архивных и удалённых"
            )
            .Produces<OrganizationMembersKpiDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<OrganizationMembersKpiDto>> HandleAsync(
        GetOrganizationMembersKpiQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return TypedResults.Ok(result);
    }
}
