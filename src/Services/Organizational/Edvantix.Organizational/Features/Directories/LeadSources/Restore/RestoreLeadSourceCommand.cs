using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.LeadSources.Restore;

/// <summary>Запрос на восстановление источника привлечения из архива.</summary>
/// <param name="Id">Идентификатор записи.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record RestoreLeadSourceCommand(Guid Id) : ICommand;

internal sealed class RestoreLeadSourceCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    ILeadSourceRepository repository
) : ICommandHandler<RestoreLeadSourceCommand>
{
    public async ValueTask<Unit> Handle(
        RestoreLeadSourceCommand command,
        CancellationToken cancellationToken
    )
    {
        var leadSource = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (leadSource is null || leadSource.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<LeadSource>(command.Id);

        var by = claims.GetProfileIdOrError();

        leadSource.Restore(by);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
