using Edvantix.Constants.Core;
using Edvantix.ProfileService.Features.EducationFeature.Models;
using FluentValidation;

namespace Edvantix.ProfileService.Features.EducationFeature;

public sealed class Validator : AbstractValidator<EducationModel>
{
    public Validator()
    {
        RuleFor(x => x.Institution)
            .NotEmpty()
            .WithMessage("Учебное заведение является обязательным полем")
            .MaximumLength(DataSchemaLength.ExtraLarge)
            .WithMessage(
                $"Название учебного заведения не должно превышать {DataSchemaLength.ExtraLarge} символов"
            );

        RuleFor(x => x.Specialty)
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Специальность не должна превышать {DataSchemaLength.Large} символов");

        RuleFor(x => x.EducationLevelId)
            .GreaterThan(0)
            .WithMessage("Уровень образования является обязательным полем");

        RuleFor(x => x.DateStart)
            .NotEmpty()
            .WithMessage("Дата начала обучения является обязательным полем");

        RuleFor(x => x.DateEnd)
            .GreaterThanOrEqualTo(x => x.DateStart)
            .When(x => x.DateEnd.HasValue)
            .WithMessage("Дата окончания обучения не может быть раньше даты начала");
    }
}
