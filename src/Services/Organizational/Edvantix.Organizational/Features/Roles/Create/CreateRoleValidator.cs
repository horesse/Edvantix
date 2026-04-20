namespace Edvantix.Organizational.Features.Roles.Create;

internal sealed class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Код роли обязателен");
    }
}
