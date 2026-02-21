using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.GroupMemberFeature.Features.AddGroupMember;

public sealed record AddGroupMemberCommand(ulong GroupId, ulong ProfileId, GroupRole Role)
    : IRequest<Guid>;

public sealed class AddGroupMemberCommandHandler(IServiceProvider provider)
    : IRequestHandler<AddGroupMemberCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        AddGroupMemberCommand request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireGroupManagementAsync(request.GroupId, cancellationToken);

        var groupRepo = provider.GetRequiredService<IGroupRepository>();
        var group =
            await groupRepo.FindByIdAsync(request.GroupId, cancellationToken)
            ?? throw new NotFoundException($"Группа с ID {request.GroupId} не найдена.");

        // Проверить, что пользователь является участником организации
        var orgMemberSpec = new OrganizationMemberSpecification(
            request.ProfileId,
            group.OrganizationId
        );
        var orgMemberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();
        var orgMember = await orgMemberRepo.FindAsync(orgMemberSpec, cancellationToken);

        if (orgMember is null)
            throw new InvalidOperationException(
                "Пользователь должен быть участником организации для добавления в группу."
            );

        // Проверить, что пользователь ещё не в группе
        var groupMemberSpec = new GroupMemberSpecification(request.ProfileId, request.GroupId);
        var groupMemberRepo = provider.GetRequiredService<IGroupMemberRepository>();
        var existing = await groupMemberRepo.FindAsync(groupMemberSpec, cancellationToken);

        if (existing is not null)
            throw new InvalidOperationException(
                "Пользователь уже является участником данной группы."
            );

        var member = new GroupMember(request.GroupId, request.ProfileId, request.Role);

        await groupMemberRepo.AddAsync(member, cancellationToken);
        await groupMemberRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return member.Id;
    }
}
