using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

namespace Edvantix.Organizational.Features.Invitations.Decline;

/// <summary>
/// Отклоняет приглашение по plaintext-токену.
/// Статус меняется на <see cref="InvitationStatus.Declined"/>.
/// </summary>
[Transactional]
public sealed record DeclineInvitationCommand(string Token) : ICommand;

internal sealed class DeclineInvitationCommandHandler(IInvitationRepository repository)
    : ICommandHandler<DeclineInvitationCommand>
{
    public async ValueTask<Unit> Handle(
        DeclineInvitationCommand command,
        CancellationToken cancellationToken
    )
    {
        var tokenHash = InvitationToken.ComputeHash(command.Token);

        var invitation =
            await repository.GetByTokenHashAsync(tokenHash, cancellationToken)
            ?? throw NotFoundException.For<Invitation>(command.Token);

        invitation.Decline();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
