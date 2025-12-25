using Edvantix.Constants.Core;
using Edvantix.Person.Features.FullName.Models;
using FluentValidation;

namespace Edvantix.Person.Features.FullName;

public sealed class FullNameModelValidator : AbstractValidator<FullNameModel>
{
    public FullNameModelValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Имя является обязательным полем")
            .MaximumLength(DataSchemaLength.Small)
            .WithMessage($"Имя не должно превышать {DataSchemaLength.Small} символов");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Фамилия является обязательным полем")
            .MaximumLength(DataSchemaLength.Small)
            .WithMessage($"Фамилия не должна превышать {DataSchemaLength.Small} символов");

        RuleFor(x => x.MiddleName)
            .MaximumLength(DataSchemaLength.Small)
            .WithMessage($"Отчество не должно превышать {DataSchemaLength.Small} символов");
    }
}
