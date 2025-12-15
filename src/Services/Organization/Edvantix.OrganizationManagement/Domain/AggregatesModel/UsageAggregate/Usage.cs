using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.UsageAggregate;

public sealed class Usage : Entity<long>, IAggregateRoot
{
    public long OrganizationId { get; set; }
    public long LimitId  { get; set; }
    
    public decimal Value { get; set; }
}
