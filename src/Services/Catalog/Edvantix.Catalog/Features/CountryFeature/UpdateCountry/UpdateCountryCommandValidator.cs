namespace Edvantix.Catalog.Features.CountryFeature.UpdateCountry;

/// <summary>
/// Валидатор команды обновления страны.
/// </summary>
internal sealed class UpdateCountryCommandValidator : AbstractValidator<UpdateCountryCommand>
{
    public UpdateCountryCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Код страны обязателен.");

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

        RuleFor(x => x.PhonePrefix)
            .NotEmpty()
            .WithMessage("Телефонный префикс обязателен.")
            .MaximumLength(10)
            .WithMessage("Телефонный префикс не должен превышать 10 символов.");

        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .WithMessage("Код валюты обязателен.")
            .Length(3)
            .WithMessage("Код валюты должен состоять из 3 символов (ISO 4217).")
            .Matches(@"^[A-Za-z]{3}$")
            .WithMessage("Код валюты должен содержать только буквы (ISO 4217).");
    }
}
