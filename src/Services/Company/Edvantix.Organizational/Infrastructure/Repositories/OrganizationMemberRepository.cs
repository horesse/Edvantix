namespace Edvantix.Organizational.Infrastructure.Repositories;

public sealed class OrganizationMemberRepository : IOrganizationMemberRepository
{
    public IUnitOfWork UnitOfWork { get; }
}
