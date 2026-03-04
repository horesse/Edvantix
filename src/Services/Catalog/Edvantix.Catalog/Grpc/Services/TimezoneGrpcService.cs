using Edvantix.Catalog.Features.TimezoneFeature.GetTimezoneByCode;
using Edvantix.Catalog.Features.TimezoneFeature.ListTimezones;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Catalog.Grpc.Services;

/// <summary>
/// gRPC-сервис для работы с часовыми поясами (IANA TZ Database).
/// Делегирует запросы в Mediator-queries; кэш работает прозрачно через <c>CachingBehavior</c>.
/// </summary>
/// <remarks>
/// TODO: авторизация межсервисных вызовов не определена командой.
/// Временно разрешён анонимный доступ; заменить на service-to-service токен или mTLS.
/// </remarks>
[AllowAnonymous]
public sealed class TimezoneGrpcService(IServiceProvider provider)
    : TimezoneService.TimezoneServiceBase
{
    /// <inheritdoc/>
    public override async Task<TimezoneResponse> GetTimezone(
        GetByCodeRequest request,
        ServerCallContext context
    )
    {
        var sender = provider.GetRequiredService<ISender>();
        var mapper = provider.GetRequiredService<IMapper<TimezoneModel, TimezoneResponse>>();

        try
        {
            var result = await sender.Send(
                new GetTimezoneByCodeQuery(request.Code),
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
    public override async Task<TimezonesResponse> ListTimezones(
        ListRequest request,
        ServerCallContext context
    )
    {
        var sender = provider.GetRequiredService<ISender>();
        var mapper = provider.GetRequiredService<IMapper<TimezoneModel, TimezoneResponse>>();

        var result = await sender.Send(
            new ListTimezonesQuery(request.ActiveOnly),
            context.CancellationToken
        );

        var response = new TimezonesResponse();
        response.Items.AddRange(mapper.Map(result));

        return response;
    }
}
