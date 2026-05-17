using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class RoomRepository(OrganizationalDbContext context) : IRoomRepository
{
    public IUnitOfWork UnitOfWork => context;

    public async Task AddAsync(Room room, CancellationToken cancellationToken = default) =>
        await context.Rooms.AddAsync(room, cancellationToken);

    public async Task<bool> ExistsAsync(
        Guid id,
        Guid organizationId,
        CancellationToken cancellationToken = default
    ) =>
        await context.Rooms.AnyAsync(
            r => r.Id == id && r.OrganizationId == organizationId && !r.IsDeleted,
            cancellationToken
        );

    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context
            .Rooms.AsTracking()
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);

    public async Task<IReadOnlyCollection<Room>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Rooms.Where(r => r.OrganizationId == organizationId && !r.IsDeleted)
            .OrderBy(r => r.Label)
            .ToListAsync(cancellationToken);
}
