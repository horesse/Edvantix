using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Features.Directories.Levels;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.Levels.GetById;

/// <summary>Получить запись справочника «Уровни» по идентификатору.</summary>
[RequirePermission(LevelPermissions.View)]
public sealed record GetLevelDirectoryByIdQuery(Guid Id) : IQuery<LevelDirectoryDto>;

internal sealed class GetLevelDirectoryByIdQueryHandler(
    ITenantContext tenantContext,
    ILevelRepository repository
) : IQueryHandler<GetLevelDirectoryByIdQuery, LevelDirectoryDto>
{
    public async ValueTask<LevelDirectoryDto> Handle(
        GetLevelDirectoryByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var level = await repository.GetByIdAsync(query.Id, cancellationToken);

        if (level is null || level.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Level>(query.Id);

        return LevelDirectoryMapper.ToDto(level);
    }
}
