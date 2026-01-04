using Edvantix.DataVault.Features.PlaygroundEntityFeature.Models;
using FluentValidation;

namespace Edvantix.DataVault.Features.PlaygroundEntityFeature.Validators;

public sealed class PlaygroundEntityValidator : AbstractValidator<PlaygroundEntityModel>
{
    public PlaygroundEntityValidator()
    {
        RuleFor(x => x.Name)
            .Must(x => !string.IsNullOrEmpty(x))
            .WithMessage("Наименование обязательно.");
    }
}
