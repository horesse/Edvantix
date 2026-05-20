using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Groups.Infrastructure.Repositories;

internal sealed class GroupRepository(GroupsDbContext context) : IGroupRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task<Group?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
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

    public async Task<IReadOnlyList<GroupStatRow>> GetStatsProjectionAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Groups.Where(g => g.OrganizationId == organizationId && !g.IsDeleted)
            .Select(g => new GroupStatRow(
                g.Status,
                g.Capacity,
                g.Members.Count(m => m.ExitedAt == null)
            ))
            .ToListAsync(cancellationToken);

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
}
