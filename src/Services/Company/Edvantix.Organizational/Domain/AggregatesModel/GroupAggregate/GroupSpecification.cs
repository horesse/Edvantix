namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Спецификация для поиска групп по идентификатору организации.
/// </summary>
public sealed class GroupSpecification : Specification<Group>
{
    public GroupSpecification(ulong organizationId)
    {
        Query.Where(x => x.OrganizationId == organizationId);
    }
}
