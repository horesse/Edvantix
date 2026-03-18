namespace Edvantix.Organizations.Features.UserRoleAssignments.AssignRole;

/// <summary>Validates the <see cref="AssignRoleCommand"/> before handling.</summary>
internal sealed class AssignRoleValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleValidator()
    {
        RuleFor(x => x.ProfileId).NotEmpty();
        RuleFor(x => x.RoleId).NotEmpty();
    }
}
