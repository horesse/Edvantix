using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Grpc.Services;

namespace Edvantix.Organizational.Features.InvitationFeature.Features.DeclineInvitation;

/// <summary>
/// Команда отклонения приглашения по токену.
/// </summary>
public sealed record DeclineInvitationCommand(Guid Token) : IRequest<Unit>;

/// <summary>
/// Обработчик отклонения приглашения.
/// </summary>
public sealed class DeclineInvitationCommandHandler(IServiceProvider provider)
    : IRequestHandler<DeclineInvitationCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        DeclineInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        var invitationRepo = provider.GetRequiredService<IInvitationRepository>();
        var spec = new InvitationSpecification(request.Token);
        var invitation =
            await invitationRepo.FindAsync(spec, cancellationToken)
            ?? throw new NotFoundException("Приглашение с указанным токеном не найдено.");

        invitation.Decline(profileId);

        await invitationRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
