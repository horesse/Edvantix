using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Edvantix.Organizational.Grpc.Services.Permission;

internal class PermissionService() : PermissionGrpcService.PermissionGrpcServiceBase
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
}
