namespace Edvantix.Catalog.Features.CountryFeature.CreateCountry;

/// <summary>
/// Валидатор команды создания страны.
/// </summary>
internal sealed class CreateCountryCommandValidator : AbstractValidator<CreateCountryCommand>
{
    public CreateCountryCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Код страны обязателен.")
            .Length(2)
            .WithMessage("Код страны должен состоять из 2 символов (ISO 3166-1 alpha-2).")
            .Matches(@"^[A-Za-z]{2}$")
            .WithMessage("Код страны должен содержать только буквы (ISO 3166-1 alpha-2).");

        RuleFor(x => x.Alpha3Code)
            .NotEmpty()
            .WithMessage("Трёхбуквенный код страны обязателен.")
            .Length(3)
            .WithMessage(
                "Трёхбуквенный код страны должен состоять из 3 символов (ISO 3166-1 alpha-3)."
            )
            .Matches(@"^[A-Za-z]{3}$")
            .WithMessage(
                "Трёхбуквенный код страны должен содержать только буквы (ISO 3166-1 alpha-3)."
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

        RuleFor(x => x.NumericCode)
            .InclusiveBetween(1, 999)
            .WithMessage("Числовой код ISO 3166-1 должен быть в диапазоне 1–999.");

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
