using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Directories.Subjects.Update;

/// <summary>Обновить данные предмета. Код предмета можно изменить с проверкой уникальности.</summary>
/// <param name="Id">Идентификатор предмета.</param>
/// <param name="Name">Новое название.</param>
/// <param name="Code">Новый код.</param>
/// <param name="Color">Новый цвет в формате <c>#RRGGBB</c>.</param>
/// <param name="Description">Новое описание.</param>
/// <param name="Order">Новый порядок сортировки.</param>
[Transactional]
[RequirePermission(SubjectPermissions.Manage)]
public sealed record UpdateSubjectCommand(
    Guid Id,
    string Name,
    string Code,
    string Color,
    string? Description,
    int Order
) : ICommand;

internal sealed class UpdateSubjectCommandHandler(
    ITenantContext tenantContext,
    ISubjectRepository repository
) : ICommandHandler<UpdateSubjectCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateSubjectCommand command,
        CancellationToken cancellationToken
    )
    {
        var orgId = tenantContext.OrganizationId;

        var subject = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (subject is null || subject.OrganizationId != orgId)
            throw NotFoundException.For<Subject>(command.Id);

        var codeVo = SubjectCode.From(command.Code);

        if (
            codeVo.Value != subject.Code.Value
            && await repository.ExistsWithCodeAsync(
                orgId,
                codeVo.Value,
                command.Id,
                cancellationToken
            )
        )
            throw new InvalidOperationException(
                $"Предмет с кодом '{codeVo.Value}' уже существует в организации."
            );

        if (
            !string.Equals(command.Name.Trim(), subject.Name, StringComparison.Ordinal)
            && await repository.ExistsWithNameAsync(
                orgId,
                command.Name,
                command.Id,
                cancellationToken
            )
        )
            throw new InvalidOperationException(
                $"Предмет с названием '{command.Name.Trim()}' уже существует в организации."
            );

        subject.Update(command.Name, codeVo, command.Color, command.Description, command.Order, Guid.Empty);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
