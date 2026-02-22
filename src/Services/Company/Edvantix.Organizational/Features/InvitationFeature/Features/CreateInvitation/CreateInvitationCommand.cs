using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.InvitationFeature.Features.CreateInvitation;

/// <summary>
/// Команда создания приглашения в организацию.
/// </summary>
public sealed record CreateInvitationCommand(
    Guid OrganizationId,
    string? InviteeEmail,
    Guid? InviteeProfileId,
    OrganizationRole Role,
    int TtlDays = Invitation.DefaultTtlDays
) : IRequest<Guid>;

/// <summary>
/// Обработчик создания приглашения.
/// Авторизация: Owner/Manager. Проверки: дубликат, уже участник, Manager не может давать Owner.
/// </summary>
public sealed class CreateInvitationCommandHandler(IServiceProvider provider)
    : IRequestHandler<CreateInvitationCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateInvitationCommand request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        var currentMember = await authService.RequireOrgRoleAsync(
            request.OrganizationId,
            cancellationToken,
            OrganizationRole.Owner,
            OrganizationRole.Manager
        );

        // Manager не может назначить роль Owner
        if (
            currentMember.Role == OrganizationRole.Manager
            && request.Role == OrganizationRole.Owner
        )
        {
            throw new ForbiddenException(
                "Менеджер не может создать приглашение с ролью владельца."
            );
        }

        var normalizedEmail = request.InviteeEmail?.Trim().ToLowerInvariant();

        // Проверить, что пользователь ещё не является участником
        if (request.InviteeProfileId.HasValue)
        {
            var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();
            var memberSpec = new OrganizationMemberSpecification(
                request.InviteeProfileId.Value,
                request.OrganizationId
            );

            var existingMember = await memberRepo.FindAsync(memberSpec, cancellationToken);
            if (existingMember is not null)
                throw new InvalidOperationException(
                    "Пользователь уже является участником данной организации."
                );
        }

        // Проверить дубликат ожидающего приглашения
        var invitationRepo = provider.GetRequiredService<IInvitationRepository>();
        var duplicateSpec = new InvitationSpecification(
            request.OrganizationId,
            normalizedEmail,
            request.InviteeProfileId
        );

        var existingInvitation = await invitationRepo.FindAsync(duplicateSpec, cancellationToken);
        if (existingInvitation is not null)
            throw new InvalidOperationException(
                "Ожидающее приглашение для данного пользователя уже существует."
            );

        var invitation = new Invitation(
            request.OrganizationId,
            currentMember.ProfileId,
            request.Role,
            normalizedEmail,
            request.InviteeProfileId,
            request.TtlDays
        );

        await invitationRepo.AddAsync(invitation, cancellationToken);
        await invitationRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return invitation.Id;
    }
}
