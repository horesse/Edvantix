namespace Edvantix.Catalog.Features.CountryFeature.UpdateCountry;

using Mediator;

/// <summary>
/// Команда обновления страны. Требует передачи всех изменяемых полей.
/// <para>
/// Логика IsActive: <c>false</c> → деактивация; <c>true</c> → активация; <c>null</c> → без изменений.
/// </para>
/// </summary>
public sealed record UpdateCountryCommand(
    string Code,
    string NameRu,
    string NameEn,
    string PhonePrefix,
    string CurrencyCode,
    bool? IsActive
) : ICommand<CountryModel>;

/// <summary>
/// Обработчик <see cref="UpdateCountryCommand"/>.
/// HTTP 404 — если страна не найдена.
/// </summary>
public sealed class UpdateCountryCommandHandler(IServiceProvider provider)
    : ICommandHandler<UpdateCountryCommand, CountryModel>
{
    /// <inheritdoc/>
    public async ValueTask<CountryModel> Handle(
        UpdateCountryCommand request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ICountryRepository>();
        var mapper = provider.GetRequiredService<IMapper<Country, CountryModel>>();

        var country =
            await repo.FindTrackedByCodeAsync(request.Code, cancellationToken)
            ?? throw new NotFoundException($"Страна с кодом '{request.Code}' не найдена.");

        if (request.IsActive == false && country.IsActive)
        {
            country.Deactivate();
        }
        else if (request.IsActive == true && !country.IsActive)
        {
            country.Activate();
        }

        country.Update(request.NameRu, request.NameEn, request.PhonePrefix, request.CurrencyCode);

        await repo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(country);
    }
}
