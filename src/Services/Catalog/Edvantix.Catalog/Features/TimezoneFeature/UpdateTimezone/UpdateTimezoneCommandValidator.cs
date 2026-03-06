namespace Edvantix.Catalog.Features.TimezoneFeature.UpdateTimezone;

/// <summary>
/// Валидатор команды обновления часового пояса.
/// </summary>
internal sealed class UpdateTimezoneCommandValidator : AbstractValidator<UpdateTimezoneCommand>
{
    public UpdateTimezoneCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Код часового пояса обязателен.");

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

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .WithMessage("Отображаемое название обязательно.")
            .MaximumLength(100)
            .WithMessage("Отображаемое название не должно превышать 100 символов.");

        RuleFor(x => x.UtcOffsetMinutes)
            .InclusiveBetween(-840, 840)
            .WithMessage("Смещение UTC должно быть в диапазоне от -840 до 840 минут.");
    }
}
