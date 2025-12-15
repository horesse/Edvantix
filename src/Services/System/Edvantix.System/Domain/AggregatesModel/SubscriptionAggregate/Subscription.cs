using Edvantix.SharedKernel.SeedWork;
using Edvantix.System.Domain.AggregatesModel.LimitAggregate;

namespace Edvantix.System.Domain.AggregatesModel.SubscriptionAggregate;

public sealed class Subscription : Entity<long>, IAggregateRoot
{
    public SubscriptionType Type { get; set; }
    
    public ICollection<Limit> Limits { get; set; } = null!;
}
