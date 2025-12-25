using Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Person.Domain.Abstractions;

public abstract class PersonalData<TIdentity> : Entity<TIdentity>
    where TIdentity : struct
{
    public long PersonInfoId { get; protected set; }
    public PersonInfo PersonInfo { get; protected set; } = null!;

    public void SetPersonInfoId(long id)
    {
        if (PersonInfoId != 0)
            throw new InvalidOperationException("PersonInfoId уже заполнен.");

        PersonInfoId = id;
    }
}
