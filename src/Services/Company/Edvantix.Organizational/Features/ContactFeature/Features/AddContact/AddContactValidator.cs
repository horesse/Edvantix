namespace Edvantix.Organizational.Features.ContactFeature.Features.AddContact;

public sealed class AddContactValidator : AbstractValidator<AddContactCommand>
{
    public AddContactValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Значение контакта обязательно.")
            .MaximumLength(500);

        RuleFor(x => x.Type).IsInEnum().WithMessage("Некорректный тип контакта.");
    }
}
