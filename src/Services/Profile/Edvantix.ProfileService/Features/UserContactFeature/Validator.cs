using Edvantix.Constants.Core;
using Edvantix.ProfileService.Features.UserContactFeature.Models;
using FluentValidation;

namespace Edvantix.ProfileService.Features.UserContactFeature;

public sealed class Validator : AbstractValidator<UserContactModel>
{
    public Validator()
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
