namespace Edvantix.Organizational.Features.Roles.Create;

internal sealed class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Название роли обязательно");
    }
}
