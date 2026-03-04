using Edvantix.Catalog.Features.RegionFeature.GetRegionByCode;
using Edvantix.Catalog.Features.RegionFeature.ListRegions;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Catalog.Grpc.Services;

/// <summary>
/// gRPC-сервис для работы с географическими регионами.
/// Делегирует запросы в Mediator-queries; кэш работает прозрачно через <c>CachingBehavior</c>.
/// </summary>
/// <remarks>
/// TODO: авторизация межсервисных вызовов не определена командой.
/// Временно разрешён анонимный доступ; заменить на service-to-service токен или mTLS.
/// </remarks>
[AllowAnonymous]
public sealed class RegionGrpcService(IServiceProvider provider) : RegionService.RegionServiceBase
{
    /// <inheritdoc/>
    public override async Task<RegionResponse> GetRegion(
        GetByCodeRequest request,
        ServerCallContext context
    )
    {
        var sender = provider.GetRequiredService<ISender>();
        var mapper = provider.GetRequiredService<IMapper<RegionModel, RegionResponse>>();

        try
        {
            var result = await sender.Send(
                new GetRegionByCodeQuery(request.Code),
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
    public override async Task<RegionsResponse> ListRegions(
        ListRequest request,
        ServerCallContext context
    )
    {
        var sender = provider.GetRequiredService<ISender>();
        var mapper = provider.GetRequiredService<IMapper<RegionModel, RegionResponse>>();

        var result = await sender.Send(
            new ListRegionsQuery(request.ActiveOnly),
            context.CancellationToken
        );

        var response = new RegionsResponse();
        response.Items.AddRange(mapper.Map(result));

        return response;
    }
}
