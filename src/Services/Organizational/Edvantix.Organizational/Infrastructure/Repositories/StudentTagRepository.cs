using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class StudentTagRepository(OrganizationalDbContext context) : IStudentTagRepository
{
    private static SpecificationEvaluator Evaluator => SpecificationEvaluator.Instance;

    public IUnitOfWork UnitOfWork => context;

    public async Task AddAsync(StudentTag studentTag, CancellationToken ct = default) =>
        await context.StudentTags.AddAsync(studentTag, ct);

    public async Task<StudentTag?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.StudentTags.AsTracking().FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<StudentTag>> ListAsync(
        ISpecification<StudentTag> specification,
        CancellationToken ct = default
    ) => await Evaluator.GetQuery(context.StudentTags.AsQueryable(), specification).ToListAsync(ct);

    public async Task<int> CountAsync(
        ISpecification<StudentTag> specification,
        CancellationToken ct = default
    ) => await Evaluator.GetQuery(context.StudentTags.AsQueryable(), specification).CountAsync(ct);

    public async Task<bool> AnyAsync(
        ISpecification<StudentTag> specification,
        CancellationToken ct = default
    ) => await Evaluator.GetQuery(context.StudentTags.AsQueryable(), specification).AnyAsync(ct);

    public async Task<DateTime?> GetLastModifiedAtAsync(
        Guid organizationId,
        CancellationToken ct = default
    ) =>
        await context
            .StudentTags.Where(t => t.OrganizationId == organizationId)
            .MaxAsync(t => (DateTime?)t.LastModifiedAt, ct);
}
