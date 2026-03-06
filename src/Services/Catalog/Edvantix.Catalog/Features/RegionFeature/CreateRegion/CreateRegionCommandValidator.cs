namespace Edvantix.Catalog.Features.RegionFeature.CreateRegion;

/// <summary>
/// Валидатор команды создания региона.
/// </summary>
internal sealed class CreateRegionCommandValidator : AbstractValidator<CreateRegionCommand>
{
    public CreateRegionCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Код региона обязателен.")
            .MaximumLength(10)
            .WithMessage("Код региона не должен превышать 10 символов.")
            .Matches(@"^[A-Za-z0-9_-]{1,10}$")
            .WithMessage(
                "Код региона должен содержать только буквы, цифры, дефис или подчёркивание."
            );

        RuleFor(x => x.NameRu)
            .NotEmpty()
            .WithMessage("Наименование на русском обязательно.")
            .MaximumLength(100)
            .WithMessage("Наименование на русском не должно превышать 100 символов.");

        RuleFor(x => x.NameEn)
            .NotEmpty()
            .WithMessage("Наименование на английском обязательно.")
            .MaximumLength(100)
            .WithMessage("Наименование на английском не должно превышать 100 символов.");
    }
}
