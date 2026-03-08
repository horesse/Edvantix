namespace Edvantix.Catalog.Features.CurrencyFeature.UpdateCurrency;
using Mediator;

/// <summary>
/// Команда обновления валюты. Требует передачи всех изменяемых полей.
/// <para>
/// Логика IsActive: <c>false</c> → деактивация; <c>true</c> → активация; <c>null</c> → без изменений.
/// </para>
/// </summary>
public sealed record UpdateCurrencyCommand(
    string Code,
    string NameRu,
    string NameEn,
    string Symbol,
    bool? IsActive
) : ICommand<CurrencyModel>;

/// <summary>
/// Обработчик <see cref="UpdateCurrencyCommand"/>.
/// HTTP 404 — если валюта не найдена.
/// </summary>
public sealed class UpdateCurrencyCommandHandler(IServiceProvider provider)
    : ICommandHandler<UpdateCurrencyCommand, CurrencyModel>
{
    /// <inheritdoc/>
    public async ValueTask<CurrencyModel> Handle(
        UpdateCurrencyCommand request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ICurrencyRepository>();
        var mapper = provider.GetRequiredService<IMapper<Currency, CurrencyModel>>();

        var currency =
            await repo.FindTrackedByCodeAsync(request.Code, cancellationToken)
            ?? throw new NotFoundException($"Валюта с кодом '{request.Code}' не найдена.");

        if (request.IsActive == false && currency.IsActive)
        {
            currency.Deactivate();
        }
        else if (request.IsActive == true && !currency.IsActive)
        {
            currency.Activate();
        }

        currency.Update(request.NameRu, request.NameEn, request.Symbol);

        await repo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(currency);
    }
}
