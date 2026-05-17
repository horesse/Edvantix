using Edvantix.Organizational.Grpc.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Edvantix.Groups.Grpc.Services;

/// <summary>
/// Обёртка над gRPC-клиентом <see cref="PermissionGrpcService.PermissionGrpcServiceClient"/>
/// с коротким in-process кешем (TTL ~45 с), чтобы не делать gRPC-вызов на каждый запрос.
/// Ключ кеша: <c>perm:{organizationId}:{profileId}:{permission}</c>.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class PermissionService(
    PermissionGrpcService.PermissionGrpcServiceClient client,
    IMemoryCache cache
) : IPermissionService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(45);

    public async Task<bool> CheckPermissionAsync(
        Guid organizationId,
        Guid profileId,
        string permission,
        CancellationToken cancellationToken
    )
    {
        var cacheKey = $"perm:{organizationId}:{profileId}:{permission}";

        if (cache.TryGetValue(cacheKey, out bool cached))
        {
            return cached;
        }

        var response = await client.CheckPermissionAsync(
            new CheckPermissionRequest
            {
                OrganizationId = organizationId.ToString(),
                ProfileId = profileId.ToString(),
                Permission = permission,
            },
            cancellationToken: cancellationToken
        );

        cache.Set(cacheKey, response.HasPermission, CacheTtl);
        return response.HasPermission;
    }
}
