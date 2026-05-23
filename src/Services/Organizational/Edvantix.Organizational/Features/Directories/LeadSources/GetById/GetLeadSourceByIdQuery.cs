using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.LeadSources.GetById;

/// <summary>Запрос получения источника привлечения по идентификатору.</summary>
/// <param name="Id">Идентификатор записи.</param>
[RequirePermission(OrganizationPermissions.View)]
public sealed record GetLeadSourceByIdQuery(Guid Id) : IQuery<LeadSourceDto>;

internal sealed class GetLeadSourceByIdQueryHandler(
    ITenantContext tenantContext,
    ILeadSourceRepository repository,
    IMapper<LeadSource, LeadSourceDto> mapper
) : IQueryHandler<GetLeadSourceByIdQuery, LeadSourceDto>
{
    public async ValueTask<LeadSourceDto> Handle(
        GetLeadSourceByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var leadSource = await repository.GetByIdAsync(query.Id, cancellationToken);

        if (leadSource is null || leadSource.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<LeadSource>(query.Id);

        return mapper.Map(leadSource);
    }
}
