namespace Edvantix.Organizations.Features.GroupMembership.GetGroupMembers;

/// <summary>Lightweight DTO for a single group membership entry.</summary>
/// <param name="ProfileId">The Persona profile identifier of the enrolled student.</param>
/// <param name="AddedAt">The timestamp when the student was added to the group.</param>
public sealed record GroupMemberDto(Guid ProfileId, DateTimeOffset AddedAt);

/// <summary>Query that returns all members of a given group.</summary>
/// <param name="GroupId">The group identifier to fetch members for.</param>
public sealed record GetGroupMembersQuery(Guid GroupId) : IQuery<List<GroupMemberDto>>;

/// <summary>
/// Returns all members of the requested group, mapped to <see cref="GroupMemberDto"/>.
/// Tenant filter is applied by the DbContext query filter on <see cref="GroupMembership"/>.
/// </summary>
public sealed class GetGroupMembersQueryHandler(IGroupRepository groupRepository)
    : IQueryHandler<GetGroupMembersQuery, List<GroupMemberDto>>
{
    /// <inheritdoc/>
    public async ValueTask<List<GroupMemberDto>> Handle(
        GetGroupMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var group =
            await groupRepository.FindByIdWithMembersAsync(request.GroupId, cancellationToken)
            ?? throw NotFoundException.For<Group>(request.GroupId);

        return group.Members
            .Select(m => new GroupMemberDto(m.ProfileId, m.AddedAt))
            .ToList();
    }
}
