namespace Edvantix.Catalog.Features.RegionFeature.UpdateRegion;

/// <summary>
/// Валидатор команды обновления региона.
/// </summary>
internal sealed class UpdateRegionCommandValidator : AbstractValidator<UpdateRegionCommand>
{
    public UpdateRegionCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Код региона обязателен.");

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
