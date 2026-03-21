namespace Edvantix.Organizations.Features.Groups.GetGroups;

/// <summary>Lightweight group summary DTO — no membership data included.</summary>
public sealed record GroupDto(Guid Id, string Name, int MaxCapacity, string Color);

/// <summary>Query that returns all groups for the current tenant.</summary>
public sealed class GetGroupsQuery : IQuery<List<GroupDto>>;

/// <summary>Returns all groups ordered by name; tenant filter is applied by the DbContext query filter.</summary>
public sealed class GetGroupsQueryHandler(IGroupRepository groupRepository)
    : IQueryHandler<GetGroupsQuery, List<GroupDto>>
{
    /// <inheritdoc/>
    public async ValueTask<List<GroupDto>> Handle(
        GetGroupsQuery request,
        CancellationToken cancellationToken
    )
    {
        var groups = await groupRepository.GetAllAsync(cancellationToken);

        return groups.Select(g => new GroupDto(g.Id, g.Name, g.MaxCapacity, g.Color)).ToList();
    }
}
