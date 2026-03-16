namespace Edvantix.Persona.Features.Profiles;

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
