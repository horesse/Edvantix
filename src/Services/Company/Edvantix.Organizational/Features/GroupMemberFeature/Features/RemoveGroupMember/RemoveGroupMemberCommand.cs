using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.GroupMemberFeature.Features.RemoveGroupMember;

public sealed record RemoveGroupMemberCommand(Guid GroupId, Guid MemberId) : IRequest<Unit>;

public sealed class RemoveGroupMemberCommandHandler(IServiceProvider provider)
    : IRequestHandler<RemoveGroupMemberCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        RemoveGroupMemberCommand request,
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

        member.Delete();
        await groupMemberRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
