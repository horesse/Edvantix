using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Builders;

namespace Edvantix.Notification.Domain.Models;

internal sealed class OutboxFilterSpec : Specification<Outbox>
{
    public OutboxFilterSpec()
    {
        Query.Where(x => x.IsSent).OrderBy(x => x.SequenceNumber);
    }
}
