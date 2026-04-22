using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

namespace Edvantix.Organizational.Features.Invitations.Accept;

/// <summary>
/// Принимает приглашение по plaintext-токену из email-ссылки или in-app уведомления.
/// Создаёт участника организации через доменное событие.
/// </summary>
[Transactional]
public sealed record AcceptInvitationCommand(string Token) : ICommand;

internal sealed class AcceptInvitationCommandHandler(
    IInvitationRepository repository,
    ClaimsPrincipal claims
) : ICommandHandler<AcceptInvitationCommand>
{
    public async ValueTask<Unit> Handle(
        AcceptInvitationCommand command,
        CancellationToken cancellationToken
    )
    {
        var tokenHash = InvitationToken.ComputeHash(command.Token);

        var invitation =
            await repository.GetByTokenHashAsync(tokenHash, cancellationToken)
            ?? throw NotFoundException.For<Invitation>(command.Token);

        var profileId = claims.GetProfileIdOrError();

        invitation.Accept(profileId);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
