namespace Edvantix.Organizational.Features.Organizations.Update;

internal sealed class UpdateOrganizationValidator : AbstractValidator<UpdateOrganizationCommand>
{
    public UpdateOrganizationValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Идентификатор организации обязателен");

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

        RuleFor(x => x.OrganizationType)
            .IsInEnum()
            .WithMessage("Указан недопустимый тип организации");

        RuleFor(x => x.LegalForm)
            .IsInEnum()
            .WithMessage("Указана недопустимая организационно-правовая форма");
    }
}
