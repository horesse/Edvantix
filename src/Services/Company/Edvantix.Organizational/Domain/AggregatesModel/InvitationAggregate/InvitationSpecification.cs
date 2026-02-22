namespace Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

public sealed class InvitationSpecification : Specification<Invitation>
{
    public InvitationSpecification(
        Guid organizationId,
        string? email = null,
        Guid? profileId = null
    )
    {
        Query
            .Where(x => x.OrganizationId == organizationId)
            .Where(x => x.Status == InvitationStatus.Pending);

        if (email is not null)
            Query.Where(x => x.InviteeEmail == email);

        if (profileId.HasValue)
            Query.Where(x => x.InviteeProfileId == profileId.Value);
    }

    public InvitationSpecification(Guid token)
    {
        Query.Where(x => x.Token == token);
    }

    public InvitationSpecification(Guid? profileId = null, Guid? organizationId = null)
    {
        Query.Where(x => x.Status == InvitationStatus.Pending);

        if (profileId.HasValue)
            Query.Where(x => x.InviteeProfileId == profileId);

        if (organizationId.HasValue)
            Query.Where(x => x.OrganizationId == organizationId);
    }
}
