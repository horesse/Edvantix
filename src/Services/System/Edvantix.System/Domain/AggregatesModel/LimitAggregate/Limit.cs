using Edvantix.SharedKernel.SeedWork;
using MassTransit.Courier.Contracts;

namespace Edvantix.System.Domain.AggregatesModel.LimitAggregate;

public sealed class Limit : Entity<long>, IAggregateRoot
{
    public long SubscriptionId { get; set; }
    public Subscription Subscription { get; set; } = null!;
    
    public LimitType Type { get; set; }
    public decimal Value { get; set; }
}
