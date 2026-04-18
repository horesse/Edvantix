namespace Edvantix.Organizational.Features.Organizations.Create;

internal sealed class CreateOrganizationValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationValidator()
    {
        RuleFor(x => x.FullLegalName)
            .NotEmpty()
            .WithMessage("Полное юридическое наименование обязательно")
            .MaximumLength(DataSchemaLength.SuperLarge)
            .WithMessage(
                $"Наименование не должно превышать {DataSchemaLength.SuperLarge} символов"
            );

        RuleFor(x => x.ShortName)
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage(
                $"Краткое наименование не должно превышать {DataSchemaLength.Large} символов"
            )
            .When(x => !string.IsNullOrEmpty(x.ShortName));

        RuleFor(x => x.RegistrationDate)
            .NotEmpty()
            .WithMessage("Дата регистрации обязательна")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Дата регистрации не может быть в будущем");

        RuleFor(x => x.LegalForm)
            .IsInEnum()
            .WithMessage("Указана недопустимая организационно-правовая форма");

        RuleFor(x => x.OrganizationType)
            .IsInEnum()
            .WithMessage("Указан недопустимый тип организации");

        RuleFor(x => x.PrimaryContactValue)
            .NotEmpty()
            .WithMessage("Значение основного контакта обязательно")
            .MaximumLength(DataSchemaLength.ExtraLarge)
            .WithMessage(
                $"Значение контакта не должно превышать {DataSchemaLength.ExtraLarge} символов"
            );

        RuleFor(x => x.PrimaryContactType)
            .IsInEnum()
            .WithMessage("Указан недопустимый тип контакта");

        RuleFor(x => x.PrimaryContactDescription)
            .NotEmpty()
            .WithMessage("Описание контакта обязательно")
            .MaximumLength(DataSchemaLength.Large)
            .WithMessage(
                $"Описание контакта не должно превышать {DataSchemaLength.Large} символов"
            );
    }
}
