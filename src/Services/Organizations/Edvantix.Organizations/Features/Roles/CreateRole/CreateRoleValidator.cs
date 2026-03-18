namespace Edvantix.Organizations.Features.Roles.CreateRole;

/// <summary>Validates <see cref="CreateRoleCommand"/> before the handler is invoked.</summary>
internal sealed class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
    }
}
