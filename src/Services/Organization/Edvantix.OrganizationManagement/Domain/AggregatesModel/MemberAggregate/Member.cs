using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.MemberAggregate;

public sealed class Member : Entity<Guid>, IAggregateRoot, ISoftDelete
{
    public long OrganizationId { get; set; }
    public Guid PersonId { get; set; }
    
    public string? Position { get; set; }
    public bool IsDeleted { get; set; }
    
    public void Delete()
    {
        IsDeleted = true;
    }
}
