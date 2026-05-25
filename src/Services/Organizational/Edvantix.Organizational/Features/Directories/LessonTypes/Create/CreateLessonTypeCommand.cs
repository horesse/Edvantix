using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.LessonTypeAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.LessonTypes.Create;

/// <summary>Создать новый тип занятия в справочнике организации.</summary>
[Transactional]
[RequirePermission(LessonTypePermissions.Manage)]
public sealed record CreateLessonTypeCommand(
    Guid OrganizationId,
    string Name,
    string Code,
    int DefaultDurationMinutes,
    string Color,
    string? Icon,
    int Order = 0
) : ICommand<LessonTypeDto>;

internal sealed class CreateLessonTypeCommandHandler(ILessonTypeRepository repository)
    : ICommandHandler<CreateLessonTypeCommand, LessonTypeDto>
{
    public async ValueTask<LessonTypeDto> Handle(
        CreateLessonTypeCommand command,
        CancellationToken cancellationToken
    )
    {
        var lessonType = new LessonType(
            command.OrganizationId,
            command.Name,
            command.Code,
            command.DefaultDurationMinutes,
            command.Color,
            command.Icon,
            command.Order
        );

        await repository.AddAsync(lessonType, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return lessonType.ToDto();
    }
}
