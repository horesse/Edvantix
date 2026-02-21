using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.GroupMemberFeature.Features.RemoveGroupMember;

public sealed record RemoveGroupMemberCommand(long GroupId, Guid MemberId) : IRequest<Unit>;

public sealed class RemoveGroupMemberCommandHandler(IServiceProvider provider)
    : IRequestHandler<RemoveGroupMemberCommand, Unit>
{
    public async Task<Unit> Handle(
        RemoveGroupMemberCommand request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireGroupManagementAsync(request.GroupId, cancellationToken);

        using var groupMemberRepo = provider.GetRequiredService<IGroupMemberRepository>();
        var member = await groupMemberRepo.GetByIdAsync(request.MemberId, cancellationToken);

        if (member is null || member.GroupId != request.GroupId)
            throw new NotFoundException($"Участник группы с ID {request.MemberId} не найден.");

        await groupMemberRepo.DeleteAsync(member, cancellationToken);
        await groupMemberRepo.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
