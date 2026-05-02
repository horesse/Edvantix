using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Invitations.List;

/// <summary>
/// Постраничный список приглашений организации.
/// </summary>
[RequirePermission(OrganizationPermissions.Members)]
public sealed record ListInvitationsQuery(
    int Offset = 0,
    int Limit = 20,
    InvitationStatus? Status = null
) : IQuery<IReadOnlyCollection<InvitationDto>>;

internal sealed class ListInvitationsQueryHandler(
    ITenantContext tenantContext,
    IInvitationRepository repository,
    IMapper<Invitation, InvitationDto> mapper
) : IQueryHandler<ListInvitationsQuery, IReadOnlyCollection<InvitationDto>>
{
    public async ValueTask<IReadOnlyCollection<InvitationDto>> Handle(
        ListInvitationsQuery query,
        CancellationToken cancellationToken
    )
    {
        var spec = new InvitationListSpecification(
            tenantContext.OrganizationId,
            query.Offset,
            query.Limit,
            query.Status
        );

        var invitations = await repository.ListAsync(spec, cancellationToken);
        return [.. invitations.Select(mapper.Map)];
    }
}
