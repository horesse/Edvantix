using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses.Create;

/// <summary>Запрос на создание статуса студента в справочнике организации.</summary>
/// <param name="Name">Отображаемое имя.</param>
/// <param name="Code">Машинный код (уникален per org).</param>
/// <param name="Tone">Визуальный тон UI.</param>
/// <param name="Order">Порядок сортировки.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record CreateStudentStatusCommand(
    string Name,
    string Code,
    StudentStatusTone Tone,
    int Order = 0
) : ICommand<StudentStatusDto>;

internal sealed class CreateStudentStatusCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IStudentStatusRepository repository,
    IMapper<StudentStatus, StudentStatusDto> mapper
) : ICommandHandler<CreateStudentStatusCommand, StudentStatusDto>
{
    public async ValueTask<StudentStatusDto> Handle(
        CreateStudentStatusCommand command,
        CancellationToken cancellationToken
    )
    {
        var createdBy = claims.GetProfileIdOrError();

        var status = new StudentStatus(
            tenantContext.OrganizationId,
            command.Name,
            command.Code,
            command.Tone,
            isSystem: false,
            command.Order,
            createdBy
        );

        await repository.AddAsync(status, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(status);
    }
}
