namespace Edvantix.Persona.Features.Profiles.UpdateContacts;

public sealed class UpdateContactsValidator : AbstractValidator<UpdateContactsCommand>
{
    public UpdateContactsValidator()
    {
        RuleForEach(x => x.Contacts).SetValidator(new ContactRequestValidator());
    }
}
