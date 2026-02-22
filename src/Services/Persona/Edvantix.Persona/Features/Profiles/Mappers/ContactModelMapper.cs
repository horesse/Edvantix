namespace Edvantix.Persona.Features.Profiles.Mappers;

/// <summary>Maps <see cref="ProfileContact"/> to <see cref="ContactModel"/>.</summary>
public sealed class ContactModelMapper : Mapper<ProfileContact, ContactModel>
{
    public override ContactModel Map(ProfileContact source) =>
        new(source.Type, source.Value, source.Description);
}
