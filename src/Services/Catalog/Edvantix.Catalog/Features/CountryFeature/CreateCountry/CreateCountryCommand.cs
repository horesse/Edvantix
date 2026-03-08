namespace Edvantix.Catalog.Features.CountryFeature.CreateCountry;

using Mediator;

/// <summary>
/// Команда создания страны.
/// </summary>
public sealed record CreateCountryCommand(
    string Code,
    string Alpha3Code,
    string NameRu,
    string NameEn,
    int NumericCode,
    string PhonePrefix,
    string CurrencyCode
) : ICommand<CountryModel>;

/// <summary>
/// Обработчик <see cref="CreateCountryCommand"/>.
/// HTTP 409 — если страна с таким кодом уже существует.
/// </summary>
public sealed class CreateCountryCommandHandler(IServiceProvider provider)
    : ICommandHandler<CreateCountryCommand, CountryModel>
{
    /// <inheritdoc/>
    public async ValueTask<CountryModel> Handle(
        CreateCountryCommand request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ICountryRepository>();
        var mapper = provider.GetRequiredService<IMapper<Country, CountryModel>>();

        var existing = await repo.GetByCodeAsync(request.Code, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException(
                $"Страна с кодом '{request.Code.ToUpperInvariant()}' уже существует."
            );
        }

        var country = new Country(
            request.Code,
            request.Alpha3Code,
            request.NameRu,
            request.NameEn,
            request.NumericCode,
            request.PhonePrefix,
            request.CurrencyCode
        );

        await repo.AddAsync(country, cancellationToken);
        await repo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(country);
    }
}
