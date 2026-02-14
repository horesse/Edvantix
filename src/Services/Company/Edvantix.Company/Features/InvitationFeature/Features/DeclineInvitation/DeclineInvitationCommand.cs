using Edvantix.Chassis.Exceptions;
using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate.Specifications;
using Edvantix.Company.Grpc.Services;
using MediatR;

namespace Edvantix.Company.Features.InvitationFeature.Features.DeclineInvitation;

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
    public async Task<Unit> Handle(
        DeclineInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        using var invitationRepo = provider.GetRequiredService<IInvitationRepository>();

        var spec = new InvitationByTokenSpecification(request.Token);
        var invitation =
            await invitationRepo.GetFirstByExpressionAsync(spec, cancellationToken)
            ?? throw new NotFoundException($"Приглашение с указанным токеном не найдено.");

        invitation.Decline(profileId);

        await invitationRepo.UpdateAsync(invitation, cancellationToken);
        await invitationRepo.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
