using Edvantix.Catalog.Features.CountryFeature.GetCountryByCode;
using Edvantix.Catalog.Features.CountryFeature.ListCountries;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Catalog.Grpc.Services;

/// <summary>
/// gRPC-сервис для работы со странами (ISO 3166-1).
/// Делегирует запросы в Mediator-queries; кэш работает прозрачно через <c>CachingBehavior</c>.
/// </summary>
/// <remarks>
/// TODO: авторизация межсервисных вызовов не определена командой.
/// Временно разрешён анонимный доступ; заменить на service-to-service токен или mTLS.
/// </remarks>
[AllowAnonymous]
public sealed class CountryGrpcService(IServiceProvider provider)
    : CountryService.CountryServiceBase
{
    /// <inheritdoc/>
    public override async Task<CountryResponse> GetCountry(
        GetByCodeRequest request,
        ServerCallContext context
    )
    {
        var sender = provider.GetRequiredService<ISender>();
        var mapper = provider.GetRequiredService<IMapper<CountryModel, CountryResponse>>();

        try
        {
            var result = await sender.Send(
                new GetCountryByCodeQuery(request.Code),
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
    public override async Task<CountriesResponse> ListCountries(
        ListRequest request,
        ServerCallContext context
    )
    {
        var sender = provider.GetRequiredService<ISender>();
        var mapper = provider.GetRequiredService<IMapper<CountryModel, CountryResponse>>();

        var result = await sender.Send(
            new ListCountriesQuery(request.ActiveOnly),
            context.CancellationToken
        );

        var response = new CountriesResponse();
        response.Items.AddRange(mapper.Map(result));

        return response;
    }
}
