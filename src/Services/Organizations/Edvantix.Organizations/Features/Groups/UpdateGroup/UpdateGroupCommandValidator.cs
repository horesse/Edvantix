namespace Edvantix.Organizations.Features.Groups.UpdateGroup;

/// <summary>Validates <see cref="UpdateGroupCommand"/> input before the command handler is invoked.</summary>
internal sealed class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupCommandValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty);

        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);

        RuleFor(x => x.MaxCapacity).GreaterThan(0);

        RuleFor(x => x.Color).NotEmpty().MaximumLength(50);
    }
}
