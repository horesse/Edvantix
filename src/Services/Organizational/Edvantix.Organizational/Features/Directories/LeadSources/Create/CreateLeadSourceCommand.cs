using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.LeadSources.Create;

/// <summary>Запрос на создание источника привлечения в справочнике организации.</summary>
/// <param name="Name">Название источника.</param>
/// <param name="Channel">Канал привлечения.</param>
/// <param name="UtmTag">UTM-метка для атрибуции (до 60 символов); <c>null</c> — не указана.</param>
/// <param name="Order">Порядок сортировки.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record CreateLeadSourceCommand(
    string Name,
    LeadChannel Channel,
    string? UtmTag,
    int Order = 0
) : ICommand<LeadSourceDto>;

internal sealed class CreateLeadSourceCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    ILeadSourceRepository repository,
    IMapper<LeadSource, LeadSourceDto> mapper
) : ICommandHandler<CreateLeadSourceCommand, LeadSourceDto>
{
    public async ValueTask<LeadSourceDto> Handle(
        CreateLeadSourceCommand command,
        CancellationToken cancellationToken
    )
    {
        var createdBy = claims.GetProfileIdOrError();

        var leadSource = new LeadSource(
            tenantContext.OrganizationId,
            command.Name,
            command.Channel,
            command.UtmTag,
            command.Order,
            createdBy
        );

        await repository.AddAsync(leadSource, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(leadSource);
    }
}
