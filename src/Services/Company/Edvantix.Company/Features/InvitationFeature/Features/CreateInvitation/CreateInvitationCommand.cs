using Edvantix.Chassis.Exceptions;
using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Company.Domain.AggregatesModel.InvitationAggregate.Specifications;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate.Specifications;
using Edvantix.Company.Services;
using MediatR;

namespace Edvantix.Company.Features.InvitationFeature.Features.CreateInvitation;

/// <summary>
/// Команда создания приглашения в организацию.
/// </summary>
public sealed record CreateInvitationCommand(
    long OrganizationId,
    string? InviteeEmail,
    long? InviteeProfileId,
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
    public async Task<Guid> Handle(
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

        // Manager не может назначить роль Owner.
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

        // Проверить, что пользователь ещё не является участником.
        if (request.InviteeProfileId.HasValue)
        {
            using var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();

            var memberSpec = new OrganizationMemberByProfileSpecification(
                request.InviteeProfileId.Value,
                request.OrganizationId
            );

            var existingMember = await memberRepo.GetFirstByExpressionAsync(
                memberSpec,
                cancellationToken
            );

            if (existingMember is not null)
                throw new InvalidOperationException(
                    "Пользователь уже является участником данной организации."
                );
        }

        // Проверить дубликат ожидающего приглашения.
        using var invitationRepo = provider.GetRequiredService<IInvitationRepository>();

        var duplicateSpec = new DuplicatePendingInvitationSpecification(
            request.OrganizationId,
            normalizedEmail,
            request.InviteeProfileId
        );

        var existingInvitation = await invitationRepo.GetFirstByExpressionAsync(
            duplicateSpec,
            cancellationToken
        );

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

        await invitationRepo.InsertAsync(invitation, cancellationToken);
        await invitationRepo.SaveEntitiesAsync(cancellationToken);

        return invitation.Id;
    }
}
