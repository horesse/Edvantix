using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.InvitationFeature.Features.CancelInvitation;

/// <summary>
/// Команда отмены приглашения (Owner/Manager).
/// </summary>
public sealed record CancelInvitationCommand(ulong OrganizationId, Guid InvitationId)
    : IRequest<Unit>;

/// <summary>
/// Обработчик отмены приглашения.
/// </summary>
public sealed class CancelInvitationCommandHandler(IServiceProvider provider)
    : IRequestHandler<CancelInvitationCommand, Unit>
{
    public async ValueTask<Unit> Handle(
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

        var invitationRepo = provider.GetRequiredService<IInvitationRepository>();
        var invitation =
            await invitationRepo.FindByIdAsync(request.InvitationId, cancellationToken)
            ?? throw new NotFoundException($"Приглашение с ID {request.InvitationId} не найдено.");

        if (invitation.OrganizationId != request.OrganizationId)
            throw new NotFoundException($"Приглашение с ID {request.InvitationId} не найдено.");

        invitation.Cancel();

        await invitationRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
