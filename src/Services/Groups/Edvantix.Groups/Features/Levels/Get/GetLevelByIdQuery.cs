using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Levels.Get;

/// <summary>Возвращает уровень по идентификатору.</summary>
[RequirePermission(LevelPermissions.View)]
public sealed record GetLevelByIdQuery(Guid Id) : IQuery<LevelDto>;

internal sealed class GetLevelByIdQueryHandler(
    ITenantContext tenantContext,
    ILevelRepository repository,
    IMapper<Level, LevelDto> mapper
) : IQueryHandler<GetLevelByIdQuery, LevelDto>
{
    public async ValueTask<LevelDto> Handle(
        GetLevelByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var level = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (level is null || level.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Level>(request.Id);

        return mapper.Map(level);
    }
}
