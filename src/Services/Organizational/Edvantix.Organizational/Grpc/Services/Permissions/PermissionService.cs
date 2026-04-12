using Edvantix.Chassis.Utilities.Guards;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Edvantix.Organizational.Grpc.Services.Permissions;

internal class PermissionService(IPermissionRepository permissionRepository)
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
        Guard.Against.NullOrWhiteSpace(request.Feature, nameof(request.Feature));

        var existing = await permissionRepository.ListAsync(
            new PermissionByFeatureSpecification(request.Feature),
            context.CancellationToken
        );

        var existingNames = existing
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var requestedNames = request
            .Permissions.Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toAdd = requestedNames
            .Where(name => !existingNames.Contains(name))
            .Select(name => new Domain.AggregatesModel.PermissionAggregate.Permission(
                request.Feature,
                name
            ))
            .ToArray();

        var toRemove = existing.Where(p => !requestedNames.Contains(p.Name)).ToArray();

        if (toAdd.Length > 0)
            await permissionRepository.AddRangeAsync(toAdd, context.CancellationToken);

        if (toRemove.Length > 0)
            permissionRepository.RemoveRange(toRemove);

        await permissionRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);

        return new SyncFeaturePermissionsResponse
        {
            Added = toAdd.Length,
            Removed = toRemove.Length,
        };
    }
}
