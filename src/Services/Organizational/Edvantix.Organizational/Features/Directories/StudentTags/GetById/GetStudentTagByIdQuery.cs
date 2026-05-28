using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.StudentTags.GetById;

/// <summary>Запрос получения тега студента по идентификатору.</summary>
/// <param name="Id">Идентификатор записи.</param>
[RequirePermission(OrganizationPermissions.View)]
public sealed record GetStudentTagByIdQuery(Guid Id) : IQuery<StudentTagDto>;

internal sealed class GetStudentTagByIdQueryHandler(
    ITenantContext tenantContext,
    IStudentTagRepository repository,
    IMapper<StudentTag, StudentTagDto> mapper
) : IQueryHandler<GetStudentTagByIdQuery, StudentTagDto>
{
    public async ValueTask<StudentTagDto> Handle(
        GetStudentTagByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var tag = await repository.GetByIdAsync(query.Id, cancellationToken);

        if (tag is null || tag.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<StudentTag>(query.Id);

        return mapper.Map(tag);
    }
}
