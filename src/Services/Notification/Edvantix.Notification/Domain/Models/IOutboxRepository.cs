using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;

namespace Edvantix.Notification.Domain.Models;

internal interface IOutboxRepository : IRepository<Outbox>
{
    Task AddAsync(Outbox outbox, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Outbox>> ListAsync(
        ISpecification<Outbox> spec,
        CancellationToken cancellationToken = default
    );

    void BulkDelete(IEnumerable<Outbox> outboxes);
}
