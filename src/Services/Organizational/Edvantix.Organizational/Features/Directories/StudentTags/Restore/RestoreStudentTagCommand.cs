using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.StudentTags.Restore;

/// <summary>Запрос на восстановление тега студента из архива.</summary>
/// <param name="Id">Идентификатор записи.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record RestoreStudentTagCommand(Guid Id) : ICommand;

internal sealed class RestoreStudentTagCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IStudentTagRepository repository
) : ICommandHandler<RestoreStudentTagCommand>
{
    public async ValueTask<Unit> Handle(
        RestoreStudentTagCommand command,
        CancellationToken cancellationToken
    )
    {
        var tag = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (tag is null || tag.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<StudentTag>(command.Id);

        var by = claims.GetProfileIdOrError();

        tag.Restore(by);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
