using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.StudentTags.Archive;

/// <summary>Запрос на архивацию тега студента.</summary>
/// <param name="Id">Идентификатор записи.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record ArchiveStudentTagCommand(Guid Id) : ICommand;

internal sealed class ArchiveStudentTagCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IStudentTagRepository repository
) : ICommandHandler<ArchiveStudentTagCommand>
{
    public async ValueTask<Unit> Handle(
        ArchiveStudentTagCommand command,
        CancellationToken cancellationToken
    )
    {
        var tag = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (tag is null || tag.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<StudentTag>(command.Id);

        var by = claims.GetProfileIdOrError();

        tag.Archive(by);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
