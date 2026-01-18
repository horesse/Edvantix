using Edvantix.DataVault.Features.PlaygroundEntityFeature.Models;
using FluentValidation;

namespace Edvantix.DataVault.Features.PlaygroundEntityFeature;

public sealed class Validator : AbstractValidator<PlaygroundEntityModel>
{
    public Validator()
    {
        RuleFor(x => x.Name)
            .Must(x => !string.IsNullOrEmpty(x))
            .WithMessage("Наименование обязательно.");
    }
}
