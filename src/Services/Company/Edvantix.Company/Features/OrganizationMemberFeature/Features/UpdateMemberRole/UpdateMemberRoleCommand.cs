using Edvantix.Chassis.Exceptions;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Services;
using MediatR;

namespace Edvantix.Company.Features.OrganizationMemberFeature.Features.UpdateMemberRole;

public sealed record UpdateMemberRoleCommand(
    long OrganizationId,
    Guid MemberId,
    OrganizationRole NewRole
) : IRequest<Unit>;

public sealed class UpdateMemberRoleCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateMemberRoleCommand, Unit>
{
    public async Task<Unit> Handle(
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

        using var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();
        var target = await memberRepo.GetByIdAsync(request.MemberId, cancellationToken);

        if (target is null || target.OrganizationId != request.OrganizationId)
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
        await memberRepo.UpdateAsync(target, cancellationToken);
        await memberRepo.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
