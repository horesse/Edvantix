using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.LeadSources.Update;

/// <summary>Запрос на обновление источника привлечения.</summary>
/// <param name="Id">Идентификатор записи (из маршрута).</param>
/// <param name="Name">Новое название.</param>
/// <param name="Channel">Новый канал привлечения.</param>
/// <param name="UtmTag">Новая UTM-метка (до 60 символов); <c>null</c> — не указана.</param>
/// <param name="Order">Новый порядок сортировки.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record UpdateLeadSourceCommand(
    Guid Id,
    string Name,
    LeadChannel Channel,
    string? UtmTag,
    int Order = 0
) : ICommand<LeadSourceDto>;

internal sealed class UpdateLeadSourceCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    ILeadSourceRepository repository,
    IMapper<LeadSource, LeadSourceDto> mapper
) : ICommandHandler<UpdateLeadSourceCommand, LeadSourceDto>
{
    public async ValueTask<LeadSourceDto> Handle(
        UpdateLeadSourceCommand command,
        CancellationToken cancellationToken
    )
    {
        var leadSource = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (leadSource is null || leadSource.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<LeadSource>(command.Id);

        var modifiedBy = claims.GetProfileIdOrError();

        leadSource.Update(command.Name, command.Channel, command.UtmTag, command.Order, modifiedBy);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(leadSource);
    }
}
