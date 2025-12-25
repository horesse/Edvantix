using Edvantix.Constants.Core;
using Edvantix.Person.Features.ContactFeature.Models;
using FluentValidation;

namespace Edvantix.Person.Features.ContactFeature;

public sealed class ContactModelValidator : AbstractValidator<ContactModel>
{
    public ContactModelValidator()
    {
        RuleFor(x => x.Type).NotEmpty().WithMessage("Тип контакта является обязательным полем");

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Значение контакта является обязательным полем");

        RuleFor(x => x.Description)
            .MaximumLength(DataSchemaLength.SuperLarge)
            .WithMessage(
                $"Описание контакта не должно превышать {DataSchemaLength.SuperLarge} символов"
            );
    }
}
