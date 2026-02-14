using Edvantix.Chassis.Exceptions;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate.Specifications;
using Edvantix.Company.Features.GroupMemberFeature.Models;
using Edvantix.Company.Services;
using MediatR;

namespace Edvantix.Company.Features.GroupMemberFeature.Features.GetGroupMembers;

public sealed record GetGroupMembersQuery(long GroupId) : IRequest<IEnumerable<GroupMemberModel>>;

public sealed class GetGroupMembersQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetGroupMembersQuery, IEnumerable<GroupMemberModel>>
{
    public async Task<IEnumerable<GroupMemberModel>> Handle(
        GetGroupMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        // Проверить, что группа существует и получить organizationId
        using var groupRepo = provider.GetRequiredService<IGroupRepository>();
        var group = await groupRepo.GetByIdAsync(request.GroupId, cancellationToken);

        if (group is null)
            throw new NotFoundException($"Группа с ID {request.GroupId} не найдена.");

        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.GetCurrentMemberAsync(group.OrganizationId, cancellationToken);

        var spec = new GroupMemberSpecification { GroupId = request.GroupId };

        using var groupMemberRepo = provider.GetRequiredService<IGroupMemberRepository>();
        var members = await groupMemberRepo.GetByExpressionAsync(spec, cancellationToken);

        // TODO: Fetch user profile data from Profile service via gRPC
        return members.Select(m => new GroupMemberModel
        {
            Id = m.Id,
            GroupId = m.GroupId,
            ProfileId = m.ProfileId,
            Role = m.Role,
            JoinedAt = m.JoinedAt,
        });
    }
}
