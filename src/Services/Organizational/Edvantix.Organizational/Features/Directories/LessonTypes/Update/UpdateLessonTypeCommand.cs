using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.LessonTypeAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.LessonTypes.Update;

/// <summary>Обновить данные типа занятия.</summary>
[Transactional]
[RequirePermission(LessonTypePermissions.Manage)]
public sealed record UpdateLessonTypeCommand(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string Code,
    int DefaultDurationMinutes,
    string Color,
    string? Icon,
    int Order = 0
) : ICommand<LessonTypeDto>;

internal sealed class UpdateLessonTypeCommandHandler(
    ITenantContext tenantContext,
    ILessonTypeRepository repository
) : ICommandHandler<UpdateLessonTypeCommand, LessonTypeDto>
{
    public async ValueTask<LessonTypeDto> Handle(
        UpdateLessonTypeCommand command,
        CancellationToken cancellationToken
    )
    {
        var lessonType = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (lessonType is null || lessonType.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<LessonType>(command.Id);

        lessonType.Update(
            command.Name,
            command.Code,
            command.DefaultDurationMinutes,
            command.Color,
            command.Icon,
            Guid.Empty
        );

        lessonType.SetOrder(command.Order, Guid.Empty);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return lessonType.ToDto();
    }
}
