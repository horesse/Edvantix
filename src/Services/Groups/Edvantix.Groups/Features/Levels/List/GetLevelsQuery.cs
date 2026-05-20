using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Levels.List;

/// <summary>
/// Возвращает список уровней организации, отсортированных по <see cref="Level.SortOrder"/>.
/// </summary>
/// <param name="IncludeInactive">Включать ли деактивированные уровни. По умолчанию — нет.</param>
[RequirePermission(LevelPermissions.View)]
public sealed record GetLevelsQuery(
    [property: Description("Включить деактивированные уровни")] bool IncludeInactive = false
) : IQuery<IReadOnlyList<LevelDto>>;

internal sealed class GetLevelsQueryHandler(
    ITenantContext tenantContext,
    ILevelRepository repository,
    IMapper<Level, LevelDto> mapper
) : IQueryHandler<GetLevelsQuery, IReadOnlyList<LevelDto>>
{
    public async ValueTask<IReadOnlyList<LevelDto>> Handle(
        GetLevelsQuery request,
        CancellationToken cancellationToken
    )
    {
        var levels = await repository.ListByOrganizationAsync(
            tenantContext.OrganizationId,
            request.IncludeInactive,
            cancellationToken
        );

        return levels.Select(mapper.Map).ToList();
    }
}
