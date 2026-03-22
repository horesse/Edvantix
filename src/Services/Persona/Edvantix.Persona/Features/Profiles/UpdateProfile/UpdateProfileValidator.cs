namespace Edvantix.Persona.Features.Profiles.UpdateProfile;

public sealed class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        const string nameRegex = "^[А-ЯЁ][а-яё]+(-[А-ЯЁ][а-яё]+)?$";

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Имя является обязательным полем")
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Имя не должно превышать {DataSchemaLength.Large} символов")
            .Matches(nameRegex)
            .WithMessage("Имя должно содержать только кириллицу, начинаться с заглавной буквы и может включать дефис");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Фамилия является обязательным полем")
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Фамилия не должна превышать {DataSchemaLength.Large} символов")
            .Matches(nameRegex)
            .WithMessage(
                "Фамилия должна содержать только кириллицу, начинаться с заглавной буквы и может включать дефис");

        RuleFor(x => x.MiddleName)
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage($"Отчество не должно превышать {DataSchemaLength.Large} символов")
            .Matches(nameRegex)
            .When(x => !string.IsNullOrEmpty(x.MiddleName))
            .WithMessage(
                "Отчество должно содержать только кириллицу, начинаться с заглавной буквы и может включать дефис");

        RuleFor(x => x.Bio)
            .MaximumLength(600)
            .WithMessage("Описание \"О себе\" не должно превышать 600 символов");

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

internal sealed class ContactRequestValidator : AbstractValidator<ContactRequest>
{
    public ContactRequestValidator()
    {
        RuleFor(x => x.Type).IsInEnum().WithMessage("Указан некорректный тип контакта");
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Значение контакта не может быть пустым")
            .MaximumLength(DataSchemaLength.ExtraLarge);
    }
}

internal sealed class EducationRequestValidator : AbstractValidator<EducationRequest>
{
    public EducationRequestValidator()
    {
        RuleFor(x => x.Institution)
            .NotEmpty()
            .WithMessage("Название учебного заведения не может быть пустым")
            .MaximumLength(DataSchemaLength.ExtraLarge);

        RuleFor(x => x.Level).IsInEnum().WithMessage("Указан некорректный уровень образования");

        RuleFor(x => x)
            .Must(e => e.DateEnd is null || e.DateEnd >= e.DateStart)
            .WithMessage("Дата окончания не может быть раньше даты начала");
    }
}

internal sealed class EmploymentHistoryRequestValidator
    : AbstractValidator<EmploymentHistoryRequest>
{
    public EmploymentHistoryRequestValidator()
    {
        RuleFor(x => x.Workplace)
            .NotEmpty()
            .WithMessage("Название компании не может быть пустым")
            .MaximumLength(DataSchemaLength.ExtraLarge);

        RuleFor(x => x.Position)
            .NotEmpty()
            .WithMessage("Должность не может быть пустой")
            .MaximumLength(DataSchemaLength.ExtraLarge);

        RuleFor(x => x)
            .Must(e => e.EndDate is null || e.EndDate >= e.StartDate)
            .WithMessage("Дата окончания не может быть раньше даты начала");
    }
}
