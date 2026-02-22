namespace Edvantix.Organizational.Features.GroupFeature.Features.CreateGroup;

public sealed class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название группы обязательно.")
            .MaximumLength(500);
    }
}
