namespace Edvantix.Organizational.Features.Roles.Update;

internal sealed class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Идентификатор роли обязателен");

        RuleFor(x => x.Name).NotEmpty().WithMessage("Название роли обязательно");
    }
}
