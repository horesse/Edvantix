namespace Edvantix.Organizations.Features.Permissions.RegisterPermissions;

/// <summary>Validates the <see cref="RegisterPermissionsCommand"/> before handling.</summary>
internal sealed class RegisterPermissionsValidator : AbstractValidator<RegisterPermissionsCommand>
{
    public RegisterPermissionsValidator()
    {
        RuleFor(x => x.PermissionNames).NotEmpty();

        RuleForEach(x => x.PermissionNames)
            .NotEmpty()
            .MaximumLength(200)
            .Matches(@"^[a-z]+\.[a-z]+-[a-z]+(-[a-z]+)*$")
            .WithMessage(
                "Permission must follow format: service.verb-noun (kebab-case), e.g. 'organizations.create-role'."
            );
    }
}
