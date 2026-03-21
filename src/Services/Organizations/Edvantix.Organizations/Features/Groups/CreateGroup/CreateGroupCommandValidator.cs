namespace Edvantix.Organizations.Features.Groups.CreateGroup;

/// <summary>Validates <see cref="CreateGroupCommand"/> input before the command handler is invoked.</summary>
internal sealed class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);

        RuleFor(x => x.MaxCapacity).GreaterThan(0);

        RuleFor(x => x.Color).NotEmpty().MaximumLength(50);
    }
}
