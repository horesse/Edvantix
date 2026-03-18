namespace Edvantix.Organizations.Features.Roles.AssignPermissions;

/// <summary>Validates <see cref="AssignPermissionsCommand"/> before the handler is invoked.</summary>
internal sealed class AssignPermissionsValidator : AbstractValidator<AssignPermissionsCommand>
{
    public AssignPermissionsValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        // PermissionNames can be empty — that is the clear-all operation
    }
}
