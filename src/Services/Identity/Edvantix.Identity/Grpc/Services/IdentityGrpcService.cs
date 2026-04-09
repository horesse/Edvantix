using Edvantix.Identity.Infrastructure.Keycloak;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Identity.Grpc.Services;

/// <summary>
/// gRPC-сервис управления учётными записями Keycloak для межсервисного взаимодействия.
/// </summary>
[AllowAnonymous]
public sealed class IdentityService(
    IKeycloakAdminService keycloakAdminService,
    ILogger<IdentityService> logger
) : IdentityGrpcService.IdentityGrpcServiceBase
{
    /// <summary>Привязывает profileId к аккаунту Keycloak.</summary>
    public override async Task<SetProfileIdReply> SetProfileId(
        SetProfileIdRequest request,
        ServerCallContext context
    )
    {
        try
        {
            await keycloakAdminService.SetProfileIdAsync(
                Guid.Parse(request.AccountId),
                Guid.Parse(request.ProfileId),
                context.CancellationToken
            );

            return new SetProfileIdReply();
        }
        catch (Exception ex) when (ex is not RpcException)
        {
            logger.LogError(ex, "gRPC SetProfileId: ошибка. Request: {Request}", request);
            throw new RpcException(new Status(StatusCode.Internal, "Внутренняя ошибка сервера."));
        }
    }

    /// <summary>Включает или отключает учётную запись Keycloak.</summary>
    public override async Task<SetUserEnabledReply> SetUserEnabled(
        SetUserEnabledRequest request,
        ServerCallContext context
    )
    {
        try
        {
            if (request.Enabled)
                await keycloakAdminService.EnableUserAsync(
                    Guid.Parse(request.AccountId),
                    context.CancellationToken
                );
            else
                await keycloakAdminService.DisableUserAsync(
                    Guid.Parse(request.AccountId),
                    context.CancellationToken
                );

            return new SetUserEnabledReply();
        }
        catch (Exception ex) when (ex is not RpcException)
        {
            logger.LogError(ex, "gRPC SetUserEnabled: ошибка. Request: {Request}", request);
            throw new RpcException(new Status(StatusCode.Internal, "Внутренняя ошибка сервера."));
        }
    }
}
