using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Enums;
using Edvantix.Organizational.Grpc.Services.Profiles;

namespace Edvantix.Organizational.Features.Invitations.Send;

/// <summary>
/// Отправляет приглашение в организацию.
/// Для <see cref="InvitationType.Email"/> требуется <see cref="Email"/>;
/// для <see cref="InvitationType.InApp"/> — <see cref="Login"/>.
/// Возвращает Id созданного приглашения.
/// </summary>
[Transactional]
[RequirePermission(OrganizationPermissions.InviteMembers)]
public sealed record SendInvitationCommand(
    InvitationType Type,
    Guid RoleId,
    DateTime ExpiresAt,
    string? Email,
    string? Login
) : ICommand<Guid>;

internal sealed class SendInvitationCommandHandler(
    ITenantContext tenantContext,
    IInvitationRepository repository,
    IProfileService profileService,
    ClaimsPrincipal claims
) : ICommandHandler<SendInvitationCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        SendInvitationCommand command,
        CancellationToken cancellationToken
    )
    {
        var inviterProfileId = claims.GetProfileIdOrError();

        Guid? inviteeProfileId = null;
        Guid? inviteeAccountId = null;

        if (command.Type == InvitationType.InApp)
        {
            var profile =
                await profileService.GetProfileByLoginAsync(command.Login!, cancellationToken)
                ?? throw NotFoundException.For<Invitation>(command.Login!);

            inviteeProfileId = Guid.Parse(profile.ProfileId);
            inviteeAccountId = Guid.Parse(profile.AccountId);
        }

        var invitation = new Invitation(
            organizationId: tenantContext.OrganizationId,
            inviterProfileId: inviterProfileId,
            roleId: command.RoleId,
            type: command.Type,
            expiresAt: command.ExpiresAt,
            email: command.Email,
            inviteeLogin: command.Login,
            inviteeProfileId: inviteeProfileId,
            inviteeAccountId: inviteeAccountId
        );

        await repository.AddAsync(invitation, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return invitation.Id;
    }
}
