using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Builders;

namespace Edvantix.Notification.Domain.Models;

internal sealed class UnsentOutboxSpec : Specification<Outbox>
{
    public UnsentOutboxSpec()
    {
        Query.AsTracking().Where(x => !x.IsSent).OrderBy(x => x.SequenceNumber);
    }
}
