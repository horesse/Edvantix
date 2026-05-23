using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class RoomRepository(OrganizationalDbContext context) : IRoomRepository
{
    private static SpecificationEvaluator Evaluator => SpecificationEvaluator.Instance;

    public IUnitOfWork UnitOfWork => context;

    public async Task AddAsync(Room room, CancellationToken ct = default) =>
        await context.Rooms.AddAsync(room, ct);

    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Rooms.AsTracking().FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<Room>> ListAsync(
        ISpecification<Room> specification,
        CancellationToken ct = default
    ) => await Evaluator.GetQuery(context.Rooms.AsQueryable(), specification).ToListAsync(ct);

    public async Task<int> CountAsync(
        ISpecification<Room> specification,
        CancellationToken ct = default
    ) => await Evaluator.GetQuery(context.Rooms.AsQueryable(), specification).CountAsync(ct);

    public async Task<bool> AnyAsync(
        ISpecification<Room> specification,
        CancellationToken ct = default
    ) => await Evaluator.GetQuery(context.Rooms.AsQueryable(), specification).AnyAsync(ct);

    public async Task<DateTime?> GetLastModifiedAtAsync(
        Guid organizationId,
        CancellationToken ct = default
    ) =>
        await context
            .Rooms.Where(r => r.OrganizationId == organizationId)
            .MaxAsync(r => (DateTime?)r.LastModifiedAt, ct);
}
