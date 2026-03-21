namespace Edvantix.Organizations.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Specification for listing groups. Applies ordering by name.
/// Tenant and soft-delete filters are applied globally by <c>OrganizationsDbContext</c>.
/// </summary>
public sealed class GroupSpecification : Specification<Group>
{
    public GroupSpecification()
    {
        Query.OrderBy(g => g.Name);
    }
}

/// <summary>
/// Specification for finding a group by ID, with optional eager loading of <see cref="Group.Members"/>.
/// Required for membership mutation operations (AddMember/RemoveMember).
/// </summary>
public sealed class GroupByIdSpecification : Specification<Group>
{
    public GroupByIdSpecification(Guid id, bool includeMembers = false)
    {
        Query.Where(g => g.Id == id);

        if (includeMembers)
            Query.Include(g => g.Members);
    }
}
