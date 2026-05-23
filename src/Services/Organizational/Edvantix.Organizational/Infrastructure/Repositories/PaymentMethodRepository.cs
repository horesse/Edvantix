using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class PaymentMethodRepository(OrganizationalDbContext context)
    : IPaymentMethodRepository
{
    private static SpecificationEvaluator Evaluator => SpecificationEvaluator.Instance;

    public IUnitOfWork UnitOfWork => context;

    public async Task AddAsync(PaymentMethod paymentMethod, CancellationToken ct = default) =>
        await context.PaymentMethods.AddAsync(paymentMethod, ct);

    public async Task<PaymentMethod?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.PaymentMethods.AsTracking().FirstOrDefaultAsync(pm => pm.Id == id, ct);

    public async Task<IReadOnlyList<PaymentMethod>> ListAsync(
        ISpecification<PaymentMethod> specification,
        CancellationToken ct = default
    ) =>
        await Evaluator
            .GetQuery(context.PaymentMethods.AsQueryable(), specification)
            .ToListAsync(ct);

    public async Task<int> CountAsync(
        ISpecification<PaymentMethod> specification,
        CancellationToken ct = default
    ) =>
        await Evaluator
            .GetQuery(context.PaymentMethods.AsQueryable(), specification)
            .CountAsync(ct);

    public async Task<bool> AnyAsync(
        ISpecification<PaymentMethod> specification,
        CancellationToken ct = default
    ) =>
        await Evaluator
            .GetQuery(context.PaymentMethods.AsQueryable(), specification)
            .AnyAsync(ct);

    public async Task<DateTime?> GetLastModifiedAtAsync(
        Guid organizationId,
        CancellationToken ct = default
    ) =>
        await context
            .PaymentMethods.Where(pm => pm.OrganizationId == organizationId)
            .MaxAsync(pm => (DateTime?)pm.LastModifiedAt, ct);
}
