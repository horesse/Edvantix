using Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Person.Domain.Abstractions;

public abstract class PersonalData : Entity<long>
{
    public long PersonInfoId { get; protected set; }
    public PersonInfo PersonInfo { get; protected set; } = null!;
}
