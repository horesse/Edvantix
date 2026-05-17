using Edvantix.Organizational.Pipelines;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Edvantix.Organizational.Grpc.Services.Permissions;

internal sealed class PermissionService(IPermissionChecker permissionChecker)
    : PermissionGrpcService.PermissionGrpcServiceBase
{
    [Authorize]
    [EnableRateLimiting("PerUserRateLimit")]
    public override async Task<CheckPermissionResponse> CheckPermission(
        CheckPermissionRequest request,
        ServerCallContext context
    )
    {
        if (!Guid.TryParse(request.OrganizationId, out var organizationId))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Некорректный organization_id.")
            );
        }

        if (!Guid.TryParse(request.ProfileId, out var profileId))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Некорректный profile_id.")
            );
        }

        if (string.IsNullOrWhiteSpace(request.Permission))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Поле permission не может быть пустым.")
            );
        }

        var result = await permissionChecker.CheckAsync(
            organizationId,
            profileId,
            request.Permission,
            context.CancellationToken
        );

        return new CheckPermissionResponse { HasPermission = result == true };
    }
}
