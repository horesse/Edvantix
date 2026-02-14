using Edvantix.Chassis.Exceptions;
using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Services;
using MediatR;

namespace Edvantix.Company.Features.InvitationFeature.Features.CancelInvitation;

/// <summary>
/// Команда отмены приглашения (Owner/Manager).
/// </summary>
public sealed record CancelInvitationCommand(long OrganizationId, Guid InvitationId)
    : IRequest<Unit>;

/// <summary>
/// Обработчик отмены приглашения.
/// </summary>
public sealed class CancelInvitationCommandHandler(IServiceProvider provider)
    : IRequestHandler<CancelInvitationCommand, Unit>
{
    public async Task<Unit> Handle(
        CancelInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireOrgRoleAsync(
            request.OrganizationId,
            cancellationToken,
            OrganizationRole.Owner,
            OrganizationRole.Manager
        );

        using var invitationRepo = provider.GetRequiredService<IInvitationRepository>();

        var invitation = await invitationRepo.GetByIdAsync(
            request.InvitationId,
            cancellationToken
        ) ?? throw new NotFoundException(
            $"Приглашение с ID {request.InvitationId} не найдено."
        );

        if (invitation.OrganizationId != request.OrganizationId)
            throw new NotFoundException(
                $"Приглашение с ID {request.InvitationId} не найдено."
            );

        invitation.Cancel();

        await invitationRepo.UpdateAsync(invitation, cancellationToken);
        await invitationRepo.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
