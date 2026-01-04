using Edvantix.Constants.Core;
using Edvantix.Person.Features.PersonInfoFeature.Models;
using FluentValidation;

namespace Edvantix.Person.Features.PersonInfoFeature;

public sealed class PersonInfoModelValidator : AbstractValidator<PersonInfoModel>
{
    public PersonInfoModelValidator()
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
