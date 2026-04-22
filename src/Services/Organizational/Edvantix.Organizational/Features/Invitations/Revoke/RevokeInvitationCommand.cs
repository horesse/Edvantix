using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Features.Invitations.Revoke;

/// <summary>
/// Отзывает приглашение по его идентификатору. Только отправитель или администратор организации.
/// </summary>
[Transactional]
[RequirePermission(OrganizationPermissions.InviteMembers)]
public sealed record RevokeInvitationCommand(Guid InvitationId) : ICommand;

internal sealed class RevokeInvitationCommandHandler(
    ITenantContext tenantContext,
    IInvitationRepository repository
) : ICommandHandler<RevokeInvitationCommand>
{
    public async ValueTask<Unit> Handle(
        RevokeInvitationCommand command,
        CancellationToken cancellationToken
    )
    {
        var invitation =
            await repository.GetByIdAsync(command.InvitationId, cancellationToken)
            ?? throw NotFoundException.For<Invitation>(command.InvitationId);

        if (invitation.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Invitation>(command.InvitationId);

        invitation.Revoke();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
