namespace Edvantix.Organizational.Features.OrganizationFeature.Features.UpdateOrganization;

public sealed class UpdateOrganizationValidator : AbstractValidator<UpdateOrganizationCommand>
{
    public UpdateOrganizationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название организации обязательно.")
            .MaximumLength(500);

        RuleFor(x => x.NameLatin)
            .NotEmpty()
            .WithMessage("Латинское название обязательно.")
            .MaximumLength(500);

        RuleFor(x => x.ShortName)
            .NotEmpty()
            .WithMessage("Краткое название обязательно.")
            .MaximumLength(200);

        RuleFor(x => x.PrintName).MaximumLength(500).When(x => x.PrintName is not null);
    }
}
