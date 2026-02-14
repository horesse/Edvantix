using FluentValidation;

namespace Edvantix.Company.Features.GroupFeature.Features.UpdateGroup;

public sealed class UpdateGroupValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название группы обязательно.")
            .MaximumLength(500);
    }
}
