using Edvantix.Constants.Core;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using FluentValidation;

namespace Edvantix.ProfileService.Features.ProfileFeature;

public sealed class ProfileModelValidator : AbstractValidator<ProfileModel>
{
    public ProfileModelValidator()
    {
        RuleFor(x => x.Gender).IsInEnum().WithMessage("Указан некорректный пол");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Имя является обязательным полем")
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Имя не должно превышать {DataSchemaLength.Large} символов");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Фамилия является обязательным полем")
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Фамилия не должна превышать {DataSchemaLength.Large} символов");

        RuleFor(x => x.MiddleName)
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Отчество не должно превышать {DataSchemaLength.Large} символов");
    }
}
