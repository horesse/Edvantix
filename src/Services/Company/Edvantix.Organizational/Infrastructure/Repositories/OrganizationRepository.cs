namespace Edvantix.Organizational.Infrastructure.Repositories;

public sealed class OrganizationRepository : IOrganizationRepository
{
    public IUnitOfWork UnitOfWork { get; }
}
