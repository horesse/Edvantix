using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.Subjects.Archive;

/// <summary>Архивировать предмет. Повторный вызов на уже архивном предмете — no-op.</summary>
[Transactional]
[RequirePermission(SubjectPermissions.Manage)]
public sealed record ArchiveSubjectCommand(Guid Id) : ICommand;

internal sealed class ArchiveSubjectCommandHandler(
    ITenantContext tenantContext,
    ISubjectRepository repository
) : ICommandHandler<ArchiveSubjectCommand>
{
    public async ValueTask<Unit> Handle(
        ArchiveSubjectCommand command,
        CancellationToken cancellationToken
    )
    {
        var subject = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (subject is null || subject.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Subject>(command.Id);

        subject.Archive(Guid.Empty);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
