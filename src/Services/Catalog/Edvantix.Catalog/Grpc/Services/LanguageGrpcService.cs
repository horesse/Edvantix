using Edvantix.Catalog.Features.LanguageFeature.GetLanguageByCode;
using Edvantix.Catalog.Features.LanguageFeature.ListLanguages;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Catalog.Grpc.Services;

/// <summary>
/// gRPC-сервис для работы с языками (ISO 639-1).
/// Делегирует запросы в Mediator-queries; кэш работает прозрачно через <c>CachingBehavior</c>.
/// </summary>
/// <remarks>
/// TODO: авторизация межсервисных вызовов не определена командой.
/// Временно разрешён анонимный доступ; заменить на service-to-service токен или mTLS.
/// </remarks>
[AllowAnonymous]
public sealed class LanguageGrpcService(IServiceProvider provider)
    : LanguageService.LanguageServiceBase
{
    /// <inheritdoc/>
    public override async Task<LanguageResponse> GetLanguage(
        GetByCodeRequest request,
        ServerCallContext context
    )
    {
        var sender = provider.GetRequiredService<ISender>();
        var mapper = provider.GetRequiredService<IMapper<LanguageModel, LanguageResponse>>();

        try
        {
            var result = await sender.Send(
                new GetLanguageByCodeQuery(request.Code),
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
    public override async Task<LanguagesResponse> ListLanguages(
        ListRequest request,
        ServerCallContext context
    )
    {
        var sender = provider.GetRequiredService<ISender>();
        var mapper = provider.GetRequiredService<IMapper<LanguageModel, LanguageResponse>>();

        var result = await sender.Send(
            new ListLanguagesQuery(request.ActiveOnly),
            context.CancellationToken
        );

        var response = new LanguagesResponse();
        response.Items.AddRange(mapper.Map(result));

        return response;
    }
}
