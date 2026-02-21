namespace Edvantix.Persona.Domain.Abstractions;

public abstract class PersonalData : Entity
{
    public ulong ProfileId { get; protected set; }
    public Profile Profile { get; protected set; } = null!;

    public void SetPersonInfoId(ulong id)
    {
        if (ProfileId != 0)
            throw new InvalidOperationException($"{nameof(ProfileId)} уже заполнен.");

        ProfileId = id;
    }
}
