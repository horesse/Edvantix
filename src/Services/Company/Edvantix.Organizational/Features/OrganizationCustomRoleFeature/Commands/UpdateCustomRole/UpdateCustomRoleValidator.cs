namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Commands.UpdateCustomRole;

/// <summary>
/// Валидатор команды обновления кастомной роли.
/// </summary>
public sealed class UpdateCustomRoleValidator : AbstractValidator<UpdateCustomRoleCommand>
{
    public UpdateCustomRoleValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.OrganizationId).NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage("Код роли может содержать только латинские буквы, цифры, '_' и '-'.");

        RuleFor(x => x.Description).MaximumLength(100).When(x => x.Description is not null);
    }
}
