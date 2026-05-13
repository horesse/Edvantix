namespace Edvantix.Organizational.Features.Groups.Stats;

public sealed class GetGroupStatsEndpoint
    : IEndpoint<Ok<GroupStatsDto>, GetGroupStatsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/groups/stats",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(new GetGroupStatsQuery(), sender, cancellationToken)
            )
            .WithName("GetGroupStats")
            .WithTags("Группы")
            .WithSummary("Получить статистику групп организации")
            .WithDescription(
                "Возвращает KPI-счётчики групп в разрезе статусов: всего, активных, на наборе, на паузе, завершённых, архивных"
            )
            .Produces<GroupStatsDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<GroupStatsDto>> HandleAsync(
        GetGroupStatsQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return TypedResults.Ok(result);
    }
}
