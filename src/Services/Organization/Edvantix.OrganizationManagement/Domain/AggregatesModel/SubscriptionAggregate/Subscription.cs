using Edvantix.OrganizationManagement.Domain.AggregatesModel.UsageAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.SubscriptionAggregate;

public sealed class Subscription : Entity<long>, IAggregateRoot
{
    public long SubscriptionId { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime? DateEnd { get; set; }
    
    public ICollection<Usage> Usages { get; set; } = null!;
}
