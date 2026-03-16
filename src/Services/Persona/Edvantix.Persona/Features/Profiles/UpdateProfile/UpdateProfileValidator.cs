namespace Edvantix.Persona.Features.Profiles.UpdateProfile;

public sealed class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
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
