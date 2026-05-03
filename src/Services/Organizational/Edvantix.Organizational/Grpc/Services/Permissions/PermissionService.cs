using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Edvantix.Organizational.Grpc.Services.Permissions;

internal sealed class PermissionService(
    IPermissionRepository permissionRepository,
    IFeatureRepository featureRepository
) : PermissionGrpcService.PermissionGrpcServiceBase
{
    [Authorize]
    [EnableRateLimiting("PerUserRateLimit")]
    public override Task<CheckPermissionResponse> CheckPermission(
        CheckPermissionRequest request,
        ServerCallContext context
    )
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Синхронизирует функциональную область и её разрешения.
    /// Апсертит Feature, затем апсертит/удаляет Permission.
    /// Идемпотентен: безопасно вызывать при каждом старте сервиса.
    /// </summary>
    public override async Task<SyncFeaturePermissionsResponse> SyncFeaturePermissions(
        SyncFeaturePermissionsRequest request,
        ServerCallContext context
    )
    {
        Guard.Against.NullOrWhiteSpace(request.ServiceCode, nameof(request.ServiceCode));
        Guard.Against.NullOrWhiteSpace(request.FeatureCode, nameof(request.FeatureCode));
        Guard.Against.NullOrWhiteSpace(request.FeatureName, nameof(request.FeatureName));

        await UpsertFeatureAsync(request, context.CancellationToken);
        var (added, removed) = await SyncPermissionsAsync(request, context.CancellationToken);

        await permissionRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);

        return new SyncFeaturePermissionsResponse { Added = added, Removed = removed };
    }

    private async Task UpsertFeatureAsync(
        SyncFeaturePermissionsRequest request,
        CancellationToken cancellationToken
    )
    {
        var feature = await featureRepository.GetByCodeAsync(
            request.FeatureCode,
            cancellationToken
        );

        if (feature is null)
            featureRepository.Add(
                new Feature(request.ServiceCode, request.FeatureCode, request.FeatureName)
            );
        else if (feature.Name != request.FeatureName)
            feature.Update(request.FeatureName);
    }

    private async Task<(int Added, int Removed)> SyncPermissionsAsync(
        SyncFeaturePermissionsRequest request,
        CancellationToken cancellationToken
    )
    {
        var all = await permissionRepository.GetAllAsync(cancellationToken);
        var existing = all.Where(p =>
                p.FeatureCode.Equals(request.FeatureCode, StringComparison.OrdinalIgnoreCase)
            )
            .ToList();

        var existingByCode = existing.ToDictionary(p => p.Code, StringComparer.OrdinalIgnoreCase);

        var desiredCodes = request
            .Permissions.Where(e => !string.IsNullOrWhiteSpace(e.Code))
            .Select(e => e.Code.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toRemove = existing.Where(p => !desiredCodes.Contains(p.Code)).ToList();
        foreach (var p in toRemove)
            permissionRepository.Remove(p);

        var added = 0;
        foreach (var entry in request.Permissions.Where(e => !string.IsNullOrWhiteSpace(e.Code)))
        {
            var code = entry.Code.Trim();
            if (existingByCode.TryGetValue(code, out var perm))
                perm.Update(entry.Name.Trim());
            else
            {
                permissionRepository.Add(
                    new Permission(request.FeatureCode, code, entry.Name.Trim())
                );
                added++;
            }
        }

        return (added, toRemove.Count);
    }
}
