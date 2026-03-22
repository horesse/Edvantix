using Edvantix.Constants.Other;

namespace Edvantix.Persona.Features.Contacts;

public sealed record ContactDto(ContactType Type, string Value, string? Description);
