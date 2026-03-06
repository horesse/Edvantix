namespace Edvantix.Catalog.Features.TimezoneFeature.CreateTimezone;

/// <summary>
/// Валидатор команды создания часового пояса.
/// </summary>
internal sealed class CreateTimezoneCommandValidator : AbstractValidator<CreateTimezoneCommand>
{
    public CreateTimezoneCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Код часового пояса обязателен.")
            .MaximumLength(50)
            .WithMessage("Код часового пояса не должен превышать 50 символов.");

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
