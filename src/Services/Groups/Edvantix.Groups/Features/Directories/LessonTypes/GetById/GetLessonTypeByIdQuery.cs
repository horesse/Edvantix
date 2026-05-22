using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.LessonTypeAggregate;
using Edvantix.Groups.Features.Directories.LessonTypes;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Directories.LessonTypes.GetById;

/// <summary>Возвращает тип занятия по идентификатору.</summary>
[RequirePermission(LessonTypePermissions.View)]
public sealed record GetLessonTypeByIdQuery(Guid Id) : IQuery<LessonTypeDto>;

internal sealed class GetLessonTypeByIdQueryHandler(
    ITenantContext tenantContext,
    ILessonTypeRepository repository
) : IQueryHandler<GetLessonTypeByIdQuery, LessonTypeDto>
{
    public async ValueTask<LessonTypeDto> Handle(
        GetLessonTypeByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var lessonType = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (lessonType is null || lessonType.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<LessonType>(request.Id);

        return lessonType.ToDto();
    }
}
