using Edvantix.Chassis.Repository;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class StudentStatusRepository(OrganizationalDbContext context)
    : IStudentStatusRepository
{
    public IUnitOfWork UnitOfWork => context;

    public async Task AddAsync(StudentStatus status, CancellationToken ct = default) =>
        await context.StudentStatuses.AddAsync(status, ct);

    public async Task AddRangeAsync(
        IEnumerable<StudentStatus> statuses,
        CancellationToken ct = default
    ) => await context.StudentStatuses.AddRangeAsync(statuses, ct);

    public async Task<StudentStatus?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context
            .StudentStatuses.AsTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<PagedResult<StudentStatus>> ListAsync(
        Guid organizationId,
        bool includeArchived,
        string? search,
        int page,
        int pageSize,
        CancellationToken ct = default
    )
    {
        var query = context
            .StudentStatuses.AsNoTracking()
            .IgnoreQueryFilters()
            .Where(s => s.OrganizationId == organizationId);

        if (!includeArchived)
            query = query.Where(s => !s.IsArchived);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(term) || s.Code.ToLower().Contains(term)
            );
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(s => s.Order)
            .ThenBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<StudentStatus>(items, page, pageSize, total);
    }

    public async Task<bool> ExistsNameAsync(
        Guid organizationId,
        string name,
        Guid? excludeId,
        CancellationToken ct = default
    ) =>
        await context.StudentStatuses.AnyAsync(
            s =>
                s.OrganizationId == organizationId
                && !s.IsArchived
                && s.Name == name
                && (excludeId == null || s.Id != excludeId.Value),
            ct
        );

    public async Task<bool> ExistsCodeAsync(
        Guid organizationId,
        string code,
        Guid? excludeId,
        CancellationToken ct = default
    ) =>
        await context.StudentStatuses.AnyAsync(
            s =>
                s.OrganizationId == organizationId
                && !s.IsArchived
                && s.Code == code
                && (excludeId == null || s.Id != excludeId.Value),
            ct
        );

    public async Task<int> CountActiveAsync(Guid organizationId, CancellationToken ct = default) =>
        await context.StudentStatuses.CountAsync(
            s => s.OrganizationId == organizationId && !s.IsArchived,
            ct
        );

    public async Task<int> CountArchivedAsync(
        Guid organizationId,
        CancellationToken ct = default
    ) =>
        await context.StudentStatuses.CountAsync(
            s => s.OrganizationId == organizationId && s.IsArchived,
            ct
        );

    public async Task<DateTime?> GetLastModifiedAtAsync(
        Guid organizationId,
        CancellationToken ct = default
    ) =>
        await context
            .StudentStatuses.Where(s => s.OrganizationId == organizationId)
            .IgnoreQueryFilters()
            .MaxAsync(s => (DateTime?)s.LastModifiedAt, ct);
}
