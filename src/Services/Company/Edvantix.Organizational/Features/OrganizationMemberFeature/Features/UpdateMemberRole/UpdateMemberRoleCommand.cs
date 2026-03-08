using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.OrganizationMemberFeature.Features.UpdateMemberRole;

public sealed record UpdateMemberRoleCommand(
    Guid OrganizationId,
    Guid MemberId,
    OrganizationRole NewRole
) : ICommand<Unit>;

public sealed class UpdateMemberRoleCommandHandler(IServiceProvider provider)
    : ICommandHandler<UpdateMemberRoleCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        UpdateMemberRoleCommand request,
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

        var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();
        var target =
            await memberRepo.FindByIdAsync(request.MemberId, cancellationToken)
            ?? throw new NotFoundException($"Участник с ID {request.MemberId} не найден.");

        if (target.OrganizationId != request.OrganizationId)
            throw new NotFoundException($"Участник с ID {request.MemberId} не найден.");

        // Manager не может назначить Owner или понизить Owner
        if (
            currentMember.Role == OrganizationRole.Manager
            && (request.NewRole == OrganizationRole.Owner || target.Role == OrganizationRole.Owner)
        )
        {
            throw new ForbiddenException(
                "Менеджер не может изменить роль владельца или назначить владельца."
            );
        }

        target.UpdateRole(request.NewRole);
        await memberRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
