using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.StudentTags.Update;

/// <summary>Запрос на обновление тега студента.</summary>
/// <param name="Id">Идентификатор записи (из маршрута).</param>
/// <param name="Name">Новое название (1–40 символов).</param>
/// <param name="Color">Новый цвет в формате HEX <c>#RRGGBB</c>.</param>
/// <param name="Order">Новый порядок сортировки.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record UpdateStudentTagCommand(Guid Id, string Name, string Color, int Order = 0)
    : ICommand<StudentTagDto>;

internal sealed class UpdateStudentTagCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IStudentTagRepository repository,
    IMapper<StudentTag, StudentTagDto> mapper
) : ICommandHandler<UpdateStudentTagCommand, StudentTagDto>
{
    public async ValueTask<StudentTagDto> Handle(
        UpdateStudentTagCommand command,
        CancellationToken cancellationToken
    )
    {
        var tag = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (tag is null || tag.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<StudentTag>(command.Id);

        var modifiedBy = claims.GetProfileIdOrError();

        tag.Update(command.Name, command.Color, command.Order, modifiedBy);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(tag);
    }
}
