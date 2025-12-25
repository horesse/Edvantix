using Edvantix.Constants.Core;
using Edvantix.Person.Features.EmploymentHistory.Models;
using FluentValidation;

namespace Edvantix.Person.Features.EmploymentHistory;

public sealed class EmploymentHistoryModelValidator : AbstractValidator<EmploymentHistoryModel>
{
    public EmploymentHistoryModelValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Название компании является обязательным полем")
            .MaximumLength(DataSchemaLength.Medium)
            .WithMessage(
                $"Название компании не должно превышать {DataSchemaLength.Medium} символов"
            );

        RuleFor(x => x.Position)
            .NotEmpty()
            .WithMessage("Должность является обязательным полем")
            .MaximumLength(DataSchemaLength.Medium)
            .WithMessage($"Должность не должна превышать {DataSchemaLength.Medium} символов");

        RuleFor(x => x.StartDate).NotEmpty().WithMessage("Дата начала является обязательным полем");

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.EndDate.HasValue)
            .WithMessage("Дата окончания не может быть раньше даты начала");

        RuleFor(x => x.Description)
            .MaximumLength(DataSchemaLength.SuperLarge)
            .WithMessage($"Описание не должно превышать {DataSchemaLength.SuperLarge} символов");
    }
}
