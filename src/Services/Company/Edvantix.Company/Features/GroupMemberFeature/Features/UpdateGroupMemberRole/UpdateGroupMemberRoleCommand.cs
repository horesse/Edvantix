using Edvantix.Chassis.Exceptions;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Company.Services;
using MediatR;

namespace Edvantix.Company.Features.GroupMemberFeature.Features.UpdateGroupMemberRole;

public sealed record UpdateGroupMemberRoleCommand(long GroupId, Guid MemberId, GroupRole NewRole)
    : IRequest<Unit>;

public sealed class UpdateGroupMemberRoleCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateGroupMemberRoleCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateGroupMemberRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireGroupManagementAsync(request.GroupId, cancellationToken);

        using var groupMemberRepo = provider.GetRequiredService<IGroupMemberRepository>();
        var member = await groupMemberRepo.GetByIdAsync(request.MemberId, cancellationToken);

        if (member is null || member.GroupId != request.GroupId)
            throw new NotFoundException($"Участник группы с ID {request.MemberId} не найден.");

        member.UpdateRole(request.NewRole);
        await groupMemberRepo.UpdateAsync(member, cancellationToken);
        await groupMemberRepo.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
