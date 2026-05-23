using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.StudentTags.Create;

/// <summary>Запрос на создание тега студента в справочнике организации.</summary>
/// <param name="Name">Название тега (1–40 символов).</param>
/// <param name="Color">Цвет метки в формате HEX <c>#RRGGBB</c>.</param>
/// <param name="Order">Порядок сортировки.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record CreateStudentTagCommand(
    string Name,
    string Color,
    int Order = 0
) : ICommand<StudentTagDto>;

internal sealed class CreateStudentTagCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IStudentTagRepository repository,
    IMapper<StudentTag, StudentTagDto> mapper
) : ICommandHandler<CreateStudentTagCommand, StudentTagDto>
{
    public async ValueTask<StudentTagDto> Handle(
        CreateStudentTagCommand command,
        CancellationToken cancellationToken
    )
    {
        var createdBy = claims.GetProfileIdOrError();

        var tag = new StudentTag(
            tenantContext.OrganizationId,
            command.Name,
            command.Color,
            command.Order,
            createdBy
        );

        await repository.AddAsync(tag, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(tag);
    }
}
