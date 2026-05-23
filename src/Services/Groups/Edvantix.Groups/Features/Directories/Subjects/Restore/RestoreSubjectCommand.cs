using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Directories.Subjects.Restore;

/// <summary>Восстановить предмет из архива. Повторный вызов на активном предмете — no-op.</summary>
[Transactional]
[RequirePermission(SubjectPermissions.Manage)]
public sealed record RestoreSubjectCommand(Guid Id) : ICommand;

internal sealed class RestoreSubjectCommandHandler(
    ITenantContext tenantContext,
    ISubjectRepository repository
) : ICommandHandler<RestoreSubjectCommand>
{
    public async ValueTask<Unit> Handle(
        RestoreSubjectCommand command,
        CancellationToken cancellationToken
    )
    {
        var subject = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (subject is null || subject.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Subject>(command.Id);

        subject.Restore(Guid.Empty);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
