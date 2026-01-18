using Edvantix.Constants.Core;
using Edvantix.ProfileService.Features.EmploymentHistoryFeature.Models;
using FluentValidation;

namespace Edvantix.ProfileService.Features.EmploymentHistoryFeature;

public sealed class Validator : AbstractValidator<EmploymentHistoryModel>
{
    public Validator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Название компании является обязательным полем")
            .MaximumLength(DataSchemaLength.ExtraLarge)
            .WithMessage(
                $"Название компании не должно превышать {DataSchemaLength.ExtraLarge} символов"
            );

        RuleFor(x => x.Position)
            .NotEmpty()
            .WithMessage("Должность является обязательным полем")
            .MaximumLength(DataSchemaLength.ExtraLarge)
            .WithMessage($"Должность не должна превышать {DataSchemaLength.ExtraLarge} символов");

        RuleFor(x => x.StartDate).NotEmpty().WithMessage("Дата начала является обязательным полем");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("Дата окончания не может быть раньше даты начала");

        RuleFor(x => x.Description)
            .MaximumLength(DataSchemaLength.Max)
            .WithMessage($"Описание не должно превышать {DataSchemaLength.Max} символов");
    }
}
