using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class GroupRepository(OrganizationalDbContext context) : IGroupRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context
            .Groups.AsTracking()
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == id && !g.IsDeleted, cancellationToken);

    public async Task<IReadOnlyCollection<Group>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Groups.Where(g => g.OrganizationId == organizationId && !g.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Group>> ListAsync(
        ISpecification<Group> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.Groups.AsQueryable(), specification)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(
        ISpecification<Group> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.Groups.AsQueryable(), specification)
            .CountAsync(cancellationToken);

    public async Task AddAsync(Group group, CancellationToken cancellationToken = default) =>
        await context.Groups.AddAsync(group, cancellationToken);

    public async Task<IReadOnlyCollection<string>> GetCodesByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    )
    {
        var codes = await context
            .Groups.Where(g => g.OrganizationId == organizationId && !g.IsDeleted)
            .Select(g => g.Code)
            .ToListAsync(cancellationToken);

        return codes.Select(c => c.Value).ToList();
    }

    public async Task<IReadOnlyDictionary<Guid, Guid>> GetTeacherProfileIdsAsync(
        IEnumerable<Guid> teacherMemberIds,
        CancellationToken cancellationToken = default
    )
    {
        var ids = teacherMemberIds.ToList();

        return await context
            .OrganizationMembers.Where(m => ids.Contains(m.Id) && !m.IsDeleted)
            .ToDictionaryAsync(m => m.Id, m => m.ProfileId, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, Room>> GetRoomsByIdsAsync(
        IEnumerable<Guid> roomIds,
        CancellationToken cancellationToken = default
    )
    {
        var ids = roomIds.ToList();

        return await context
            .Rooms.Where(r => ids.Contains(r.Id) && !r.IsDeleted)
            .ToDictionaryAsync(r => r.Id, r => r, cancellationToken);
    }
}
