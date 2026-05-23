using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses.Archive;

/// <summary>Запрос на архивацию статуса студента.</summary>
/// <param name="Id">Идентификатор записи.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record ArchiveStudentStatusCommand(Guid Id) : ICommand;

internal sealed class ArchiveStudentStatusCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IStudentStatusRepository repository
) : ICommandHandler<ArchiveStudentStatusCommand>
{
    public async ValueTask<Unit> Handle(
        ArchiveStudentStatusCommand command,
        CancellationToken cancellationToken
    )
    {
        var status = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (status is null || status.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<StudentStatus>(command.Id);

        var by = claims.GetProfileIdOrError();

        // Выбросит InvalidOperationException (→ 409) если IsSystem=true
        status.Archive(by);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
