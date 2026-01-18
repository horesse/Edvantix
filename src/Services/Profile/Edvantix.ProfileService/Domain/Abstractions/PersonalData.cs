using Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.ProfileService.Domain.Abstractions;

public abstract class PersonalData<TIdentity> : Entity<TIdentity>
    where TIdentity : struct
{
    public long ProfileId { get; protected set; }
    public Profile Profile { get; protected set; } = null!;

    public void SetPersonInfoId(long id)
    {
        if (ProfileId != 0)
            throw new InvalidOperationException($"{nameof(ProfileId)} уже заполнен.");

        ProfileId = id;
    }
}
