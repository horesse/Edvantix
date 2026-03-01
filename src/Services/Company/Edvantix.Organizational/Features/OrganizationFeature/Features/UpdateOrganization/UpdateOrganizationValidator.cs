using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

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

        RuleFor(x => x.OrganizationType)
            .IsInEnum()
            .WithMessage("Укажите корректный тип организации.");

        RuleFor(x => x.LegalFormId)
            .NotEmpty()
            .WithMessage("Организационно-правовая форма обязательна.");
    }
}
