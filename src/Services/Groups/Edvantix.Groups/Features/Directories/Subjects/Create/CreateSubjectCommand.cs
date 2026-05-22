using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Directories.Subjects.Create;

/// <summary>Создать новый предмет в справочнике организации.</summary>
/// <param name="Name">Отображаемое название.</param>
/// <param name="Code">Уникальный код (A-Z0-9, max 10 символов).</param>
/// <param name="Color">Цвет в формате <c>#RRGGBB</c>.</param>
/// <param name="Description">Описание предмета (опционально).</param>
/// <param name="Order">Порядок сортировки в UI.</param>
[Transactional]
[RequirePermission(SubjectPermissions.Manage)]
public sealed record CreateSubjectCommand(
    string Name,
    string Code,
    string Color = Subject.DefaultColor,
    string? Description = null,
    int Order = 0
) : ICommand<Guid>;

internal sealed class CreateSubjectCommandHandler(
    ITenantContext tenantContext,
    ISubjectRepository repository
) : ICommandHandler<CreateSubjectCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateSubjectCommand command,
        CancellationToken cancellationToken
    )
    {
        var orgId = tenantContext.OrganizationId;
        var codeVo = SubjectCode.From(command.Code);

        if (await repository.ExistsWithCodeAsync(orgId, codeVo.Value, cancellationToken: cancellationToken))
            throw new InvalidOperationException(
                $"Предмет с кодом '{codeVo.Value}' уже существует в организации."
            );

        if (await repository.ExistsWithNameAsync(orgId, command.Name, cancellationToken: cancellationToken))
            throw new InvalidOperationException(
                $"Предмет с названием '{command.Name.Trim()}' уже существует в организации."
            );

        var subject = new Subject(
            orgId,
            command.Name,
            codeVo,
            command.Color,
            command.Description,
            command.Order
        );

        await repository.AddAsync(subject, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return subject.Id;
    }
}
