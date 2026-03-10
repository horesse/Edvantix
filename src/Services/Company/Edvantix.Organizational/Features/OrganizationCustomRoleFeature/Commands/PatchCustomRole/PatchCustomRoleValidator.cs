namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Commands.PatchCustomRole;

/// <summary>
/// Валидатор команды частичного обновления кастомной роли.
/// </summary>
public sealed class PatchCustomRoleValidator : AbstractValidator<PatchCustomRoleCommand>
{
    public PatchCustomRoleValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(100).When(x => x.Description is not null);
    }
}
