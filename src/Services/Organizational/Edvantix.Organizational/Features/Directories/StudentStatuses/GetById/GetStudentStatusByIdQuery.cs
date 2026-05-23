using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses.GetById;

/// <summary>Запрос получения статуса студента по идентификатору.</summary>
/// <param name="Id">Идентификатор записи.</param>
[RequirePermission(OrganizationPermissions.View)]
public sealed record GetStudentStatusByIdQuery(Guid Id) : IQuery<StudentStatusDto>;

internal sealed class GetStudentStatusByIdQueryHandler(
    ITenantContext tenantContext,
    IStudentStatusRepository repository,
    IMapper<StudentStatus, StudentStatusDto> mapper
) : IQueryHandler<GetStudentStatusByIdQuery, StudentStatusDto>
{
    public async ValueTask<StudentStatusDto> Handle(
        GetStudentStatusByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var status = await repository.GetByIdAsync(query.Id, cancellationToken);

        if (status is null || status.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<StudentStatus>(query.Id);

        return mapper.Map(status);
    }
}
