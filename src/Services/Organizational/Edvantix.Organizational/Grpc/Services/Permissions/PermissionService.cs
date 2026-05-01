using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Edvantix.Organizational.Grpc.Services.Permissions;

internal sealed class PermissionService(IFeatureRepository featureRepository)
    : PermissionGrpcService.PermissionGrpcServiceBase
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

    public override async Task<SyncFeaturePermissionsResponse> SyncFeaturePermissions(
        SyncFeaturePermissionsRequest request,
        ServerCallContext context
    )
    {
        Guard.Against.NullOrWhiteSpace(request.FeatureCode, nameof(request.FeatureCode));
        Guard.Against.NullOrWhiteSpace(request.FeatureName, nameof(request.FeatureName));

        var feature = await featureRepository.GetByCodeAsync(
            request.FeatureCode,
            context.CancellationToken
        );

        if (feature is null)
        {
            feature = new Feature(request.FeatureCode, request.FeatureName);
            featureRepository.Add(feature);
        }
        else if (feature.Name != request.FeatureName)
        {
            feature.UpdateName(request.FeatureName);
        }

        var desired = request
            .Permissions.Where(e => !string.IsNullOrWhiteSpace(e.Code))
            .Select(e => (Code: e.Code.Trim(), Name: e.Name.Trim()));

        var (added, removed) = feature.SyncPermissions(desired);

        await featureRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);

        return new SyncFeaturePermissionsResponse { Added = added, Removed = removed };
    }
}
