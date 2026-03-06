using Edvantix.Catalog.Features.CurrencyFeature.GetCurrencyByCode;
using Edvantix.Catalog.Features.CurrencyFeature.ListCurrencies;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Catalog.Grpc.Services;

/// <summary>
/// gRPC-сервис для работы с валютами (ISO 4217).
/// Делегирует запросы в Mediator-queries; кэш работает прозрачно через <c>CachingBehavior</c>.
/// </summary>
/// <remarks>
/// TODO: авторизация межсервисных вызовов не определена командой.
/// Временно разрешён анонимный доступ; заменить на service-to-service токен или mTLS.
/// </remarks>
[AllowAnonymous]
public sealed class CurrencyGrpcService(IServiceProvider provider)
    : CurrencyService.CurrencyServiceBase
{
    /// <inheritdoc/>
    public override async Task<CurrencyResponse> GetCurrency(
        GetByCodeRequest request,
        ServerCallContext context
    )
    {
        var sender = provider.GetRequiredService<ISender>();
        var mapper = provider.GetRequiredService<IMapper<CurrencyModel, CurrencyResponse>>();

        try
        {
            var result = await sender.Send(
                new GetCurrencyByCodeQuery(request.Code),
                context.CancellationToken
            );

            return mapper.Map(result);
        }
        catch (NotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
    }

    /// <inheritdoc/>
    public override async Task<CurrenciesResponse> ListCurrencies(
        ListRequest request,
        ServerCallContext context
    )
    {
        var sender = provider.GetRequiredService<ISender>();
        var mapper = provider.GetRequiredService<IMapper<CurrencyModel, CurrencyResponse>>();

        var result = await sender.Send(
            new ListCurrenciesQuery(request.ActiveOnly),
            context.CancellationToken
        );

        var response = new CurrenciesResponse();
        response.Items.AddRange(mapper.Map(result));

        return response;
    }
}
