namespace Edvantix.Organizations.Features.Roles.UpdateRole;

/// <summary>Validates <see cref="UpdateRoleCommand"/> before the handler is invoked.</summary>
internal sealed class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
    }
}
