namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Спецификация для поиска групп по идентификатору организации.
/// </summary>
public sealed class GroupSpecification : Specification<Group>
{
    public GroupSpecification(Guid organizationId, bool includeMembers = false)
    {
        Query.Where(x => x.OrganizationId == organizationId);

        if (includeMembers)
            Query.Include(x => x.Members);
    }
}
