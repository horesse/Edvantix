using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.GroupMemberFeature.Features.UpdateGroupMemberRole;

public sealed record UpdateGroupMemberRoleCommand(Guid GroupId, Guid MemberId, GroupRole NewRole)
    : IRequest<Unit>;

public sealed class UpdateGroupMemberRoleCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateGroupMemberRoleCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        UpdateGroupMemberRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireGroupManagementAsync(request.GroupId, cancellationToken);

        var groupMemberRepo = provider.GetRequiredService<IGroupMemberRepository>();
        var member =
            await groupMemberRepo.FindByIdAsync(request.MemberId, cancellationToken)
            ?? throw new NotFoundException($"Участник группы с ID {request.MemberId} не найден.");

        if (member.GroupId != request.GroupId)
            throw new NotFoundException($"Участник группы с ID {request.MemberId} не найден.");

        member.UpdateRole(request.NewRole);
        await groupMemberRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
