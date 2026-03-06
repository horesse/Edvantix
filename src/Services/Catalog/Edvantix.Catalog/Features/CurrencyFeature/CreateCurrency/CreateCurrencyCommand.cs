namespace Edvantix.Catalog.Features.CurrencyFeature.CreateCurrency;

/// <summary>
/// Команда создания новой валюты в справочнике.
/// После успешного сохранения EF-interceptor публикует <see cref="CatalogEntryChangedEvent"/>,
/// который инвалидирует кэш через отдельный handler (EDV-77).
/// </summary>
public sealed record CreateCurrencyCommand(
    string Code,
    string NameRu,
    string NameEn,
    string Symbol,
    int NumericCode,
    int DecimalDigits
) : IRequest<CurrencyModel>;

/// <summary>
/// Обработчик <see cref="CreateCurrencyCommand"/>.
/// HTTP 409 — при попытке создать валюту с уже существующим кодом.
/// </summary>
public sealed class CreateCurrencyCommandHandler(IServiceProvider provider)
    : IRequestHandler<CreateCurrencyCommand, CurrencyModel>
{
    /// <inheritdoc/>
    public async ValueTask<CurrencyModel> Handle(
        CreateCurrencyCommand request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ICurrencyRepository>();
        var mapper = provider.GetRequiredService<IMapper<Currency, CurrencyModel>>();

        // Проверяем дублирование кода (читаем без трекинга — только для проверки)
        var existing = await repo.GetByCodeAsync(request.Code, cancellationToken);

        if (existing is not null)
        {
            throw new InvalidOperationException(
                $"Валюта с кодом '{request.Code.ToUpperInvariant()}' уже существует."
            );
        }

        var currency = new Currency(
            request.Code,
            request.NameRu,
            request.NameEn,
            request.Symbol,
            request.NumericCode,
            request.DecimalDigits
        );

        await repo.AddAsync(currency, cancellationToken);
        await repo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(currency);
    }
}
