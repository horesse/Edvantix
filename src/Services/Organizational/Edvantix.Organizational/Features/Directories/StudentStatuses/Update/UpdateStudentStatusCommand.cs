using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses.Update;

/// <summary>Запрос на обновление статуса студента.</summary>
/// <param name="Id">Идентификатор записи (из маршрута).</param>
/// <param name="Name">Новое имя.</param>
/// <param name="Code">Новый машинный код.</param>
/// <param name="Tone">Новый визуальный тон.</param>
/// <param name="Order">Новый порядок сортировки.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record UpdateStudentStatusCommand(
    Guid Id,
    string Name,
    string Code,
    StudentStatusTone Tone,
    int Order = 0
) : ICommand<StudentStatusDto>;

internal sealed class UpdateStudentStatusCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IStudentStatusRepository repository,
    IMapper<StudentStatus, StudentStatusDto> mapper
) : ICommandHandler<UpdateStudentStatusCommand, StudentStatusDto>
{
    public async ValueTask<StudentStatusDto> Handle(
        UpdateStudentStatusCommand command,
        CancellationToken cancellationToken
    )
    {
        var status = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (status is null || status.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<StudentStatus>(command.Id);

        var modifiedBy = claims.GetProfileIdOrError();

        status.Update(command.Name, command.Code, command.Tone, command.Order, modifiedBy);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(status);
    }
}
