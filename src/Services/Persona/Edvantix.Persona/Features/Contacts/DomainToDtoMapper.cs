namespace Edvantix.Persona.Features.Contacts;

public sealed class DomainToDtoMapper : Mapper<ProfileContact, ContactDto>
{
    public override ContactDto Map(ProfileContact source) =>
        new(source.Type, source.Value, source.Description);
}
