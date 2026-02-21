namespace Edvantix.Organizational.Infrastructure.Repositories;

public sealed class GroupMemberRepository : IGroupMemberRepository
{
    public IUnitOfWork UnitOfWork { get; }
}
