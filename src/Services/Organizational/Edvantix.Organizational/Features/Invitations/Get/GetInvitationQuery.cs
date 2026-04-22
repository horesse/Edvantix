using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Features.Invitations.Get;

/// <summary>Возвращает приглашение по идентификатору.</summary>
[RequirePermission(OrganizationPermissions.InviteMembers)]
public sealed record GetInvitationQuery(Guid Id) : IQuery<InvitationDto>;

internal sealed class GetInvitationQueryHandler(
    ITenantContext tenantContext,
    IInvitationRepository repository,
    IMapper<Invitation, InvitationDto> mapper
) : IQueryHandler<GetInvitationQuery, InvitationDto>
{
    public async ValueTask<InvitationDto> Handle(
        GetInvitationQuery query,
        CancellationToken cancellationToken
    )
    {
        var invitation =
            await repository.GetByIdAsync(query.Id, cancellationToken)
            ?? throw NotFoundException.For<Invitation>(query.Id);

        if (invitation.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Invitation>(query.Id);

        return mapper.Map(invitation);
    }
}
