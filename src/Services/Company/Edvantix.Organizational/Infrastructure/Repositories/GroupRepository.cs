namespace Edvantix.Organizational.Infrastructure.Repositories;

public sealed class GroupRepository : IGroupRepository
{
    public IUnitOfWork UnitOfWork { get; }
}
