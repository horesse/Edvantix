using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate.Specifications;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.GroupMemberFeature.Features.AddGroupMember;

public sealed record AddGroupMemberCommand(long GroupId, int ProfileId, GroupRole Role)
    : IRequest<Guid>;

public sealed class AddGroupMemberCommandHandler(IServiceProvider provider)
    : IRequestHandler<AddGroupMemberCommand, Guid>
{
    public async Task<Guid> Handle(
        AddGroupMemberCommand request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireGroupManagementAsync(request.GroupId, cancellationToken);

        // Проверить, что группа существует
        using var groupRepo = provider.GetRequiredService<IGroupRepository>();
        var group = await groupRepo.GetByIdAsync(request.GroupId, cancellationToken);

        if (group is null)
            throw new NotFoundException($"Группа с ID {request.GroupId} не найдена.");

        // Проверить, что пользователь является участником организации
        var orgMemberSpec = new OrganizationMemberByProfileSpecification(
            request.ProfileId,
            group.OrganizationId
        );
        using var orgMemberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();
        var orgMember = await orgMemberRepo.GetFirstByExpressionAsync(
            orgMemberSpec,
            cancellationToken
        );

        if (orgMember is null)
            throw new InvalidOperationException(
                "Пользователь должен быть участником организации для добавления в группу."
            );

        // Проверить, что пользователь ещё не в группе
        var groupMemberSpec = new GroupMemberByProfileSpecification(
            request.ProfileId,
            request.GroupId
        );
        using var groupMemberRepo = provider.GetRequiredService<IGroupMemberRepository>();
        var existing = await groupMemberRepo.GetFirstByExpressionAsync(
            groupMemberSpec,
            cancellationToken
        );

        if (existing is not null)
            throw new InvalidOperationException(
                "Пользователь уже является участником данной группы."
            );

        var member = new GroupMember(request.GroupId, request.ProfileId, request.Role);

        await groupMemberRepo.InsertAsync(member, cancellationToken);
        await groupMemberRepo.SaveEntitiesAsync(cancellationToken);

        return member.Id;
    }
}
