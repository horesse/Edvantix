using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class StudentStatusRepository(OrganizationalDbContext context)
    : IStudentStatusRepository
{
    private static SpecificationEvaluator Evaluator => SpecificationEvaluator.Instance;

    public IUnitOfWork UnitOfWork => context;

    public async Task AddAsync(StudentStatus status, CancellationToken ct = default) =>
        await context.StudentStatuses.AddAsync(status, ct);

    public async Task AddRangeAsync(
        IEnumerable<StudentStatus> statuses,
        CancellationToken ct = default
    ) => await context.StudentStatuses.AddRangeAsync(statuses, ct);

    public async Task<StudentStatus?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.StudentStatuses.AsTracking().FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<StudentStatus>> ListAsync(
        ISpecification<StudentStatus> specification,
        CancellationToken ct = default
    ) =>
        await Evaluator
            .GetQuery(context.StudentStatuses.AsQueryable(), specification)
            .ToListAsync(ct);

    public async Task<int> CountAsync(
        ISpecification<StudentStatus> specification,
        CancellationToken ct = default
    ) =>
        await Evaluator
            .GetQuery(context.StudentStatuses.AsQueryable(), specification)
            .CountAsync(ct);

    public async Task<bool> AnyAsync(
        ISpecification<StudentStatus> specification,
        CancellationToken ct = default
    ) =>
        await Evaluator
            .GetQuery(context.StudentStatuses.AsQueryable(), specification)
            .AnyAsync(ct);

    public async Task<DateTime?> GetLastModifiedAtAsync(
        Guid organizationId,
        CancellationToken ct = default
    ) =>
        await context
            .StudentStatuses.Where(s => s.OrganizationId == organizationId)
            .MaxAsync(s => (DateTime?)s.LastModifiedAt, ct);
}
