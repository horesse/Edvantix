using Edvantix.Persona.Features.Profiles.Update;

namespace Edvantix.Persona.Features.Admin.Profiles.Edit;

public sealed class AdminUpdateProfileValidator : AbstractValidator<AdminUpdateProfileCommand>
{
    public AdminUpdateProfileValidator()
    {
        const string nameRegex = "^[А-ЯЁ][а-яё]+(-[А-ЯЁ][а-яё]+)?$";

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Имя является обязательным полем")
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Имя не должно превышать {DataSchemaLength.Large} символов")
            .Matches(nameRegex)
            .WithMessage(
                "Имя должно содержать только кириллицу, начинаться с заглавной буквы и может включать дефис"
            );

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Фамилия является обязательным полем")
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Фамилия не должна превышать {DataSchemaLength.Large} символов")
            .Matches(nameRegex)
            .WithMessage(
                "Фамилия должна содержать только кириллицу, начинаться с заглавной буквы и может включать дефис"
            );

        RuleFor(x => x.MiddleName)
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Отчество не должно превышать {DataSchemaLength.Large} символов")
            .Matches(nameRegex)
            .When(x => !string.IsNullOrEmpty(x.MiddleName))
            .WithMessage(
                "Отчество должно содержать только кириллицу, начинаться с заглавной буквы и может включать дефис"
            );

        RuleFor(x => x.Bio)
            .MaximumLength(600)
            .WithMessage("Описание \"О себе\" не должно превышать 600 символов");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Причина изменения является обязательным полем")
            .MaximumLength(500)
            .WithMessage("Причина изменения не должна превышать 500 символов");

        RuleFor(x => x.Skills)
            .Must(s => s.Count <= Profile.MaxSkillsCount)
            .WithMessage($"Нельзя добавить более {Profile.MaxSkillsCount} навыков");

        RuleForEach(x => x.Skills)
            .NotEmpty()
            .WithMessage("Название навыка не может быть пустым")
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Название навыка не должно превышать {DataSchemaLength.Large} символов");

        RuleForEach(x => x.Contacts).SetValidator(new ContactRequestValidator());
        RuleForEach(x => x.Educations).SetValidator(new EducationRequestValidator());
        RuleForEach(x => x.EmploymentHistories)
            .SetValidator(new EmploymentHistoryRequestValidator());
    }
}
