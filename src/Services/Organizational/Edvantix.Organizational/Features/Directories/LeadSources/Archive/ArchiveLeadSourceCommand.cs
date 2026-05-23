using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.LeadSources.Archive;

/// <summary>Запрос на архивацию источника привлечения.</summary>
/// <param name="Id">Идентификатор записи.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record ArchiveLeadSourceCommand(Guid Id) : ICommand;

internal sealed class ArchiveLeadSourceCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    ILeadSourceRepository repository
) : ICommandHandler<ArchiveLeadSourceCommand>
{
    public async ValueTask<Unit> Handle(
        ArchiveLeadSourceCommand command,
        CancellationToken cancellationToken
    )
    {
        var leadSource = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (leadSource is null || leadSource.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<LeadSource>(command.Id);

        var by = claims.GetProfileIdOrError();

        leadSource.Archive(by);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
