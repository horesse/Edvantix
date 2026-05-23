using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Directories.Subjects.GetById;

/// <summary>Возвращает предмет по идентификатору.</summary>
[RequirePermission(SubjectPermissions.View)]
public sealed record GetSubjectByIdQuery(Guid Id) : IQuery<SubjectDto>;

internal sealed class GetSubjectByIdQueryHandler(
    ITenantContext tenantContext,
    ISubjectRepository repository,
    IMapper<Subject, SubjectDto> mapper
) : IQueryHandler<GetSubjectByIdQuery, SubjectDto>
{
    public async ValueTask<SubjectDto> Handle(
        GetSubjectByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var subject = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (subject is null || subject.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Subject>(request.Id);

        return mapper.Map(subject);
    }
}
