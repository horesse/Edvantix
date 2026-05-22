using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses.Restore;

/// <summary>Запрос на восстановление статуса студента из архива.</summary>
/// <param name="Id">Идентификатор записи.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record RestoreStudentStatusCommand(Guid Id) : ICommand;

internal sealed class RestoreStudentStatusCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IStudentStatusRepository repository
) : ICommandHandler<RestoreStudentStatusCommand>
{
    public async ValueTask<Unit> Handle(
        RestoreStudentStatusCommand command,
        CancellationToken cancellationToken
    )
    {
        var status = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (status is null || status.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<StudentStatus>(command.Id);

        var by = claims.GetProfileIdOrError();

        // Выбросит InvalidOperationException (→ 409) если IsSystem=true
        status.Restore(by);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
