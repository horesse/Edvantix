using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Security.Tenant;
using Edvantix.Organizations.Grpc.Generated;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Edvantix.Chassis.Security.Authorization;

/// <summary>
/// Resolves permission checks by calling the Organizations gRPC <c>CheckPermission</c> endpoint.
/// Extracts the user identity from the <c>profile_id</c> claim and the school from
/// <see cref="ITenantContext"/> (populated by <see cref="TenantMiddleware"/> from the
/// <c>X-School-Id</c> request header).
/// </summary>
internal sealed class PermissionRequirementHandler(
    PermissionsGrpcService.PermissionsGrpcServiceClient grpcClient,
    IHttpContextAccessor httpContextAccessor,
    ILogger<PermissionRequirementHandler> logger
) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement
    )
    {
        // Extract profile_id from claims — identifies the user within the school.
        var profileIdClaim = context.User.FindFirst(KeycloakClaimTypes.Profile);
        if (profileIdClaim is null || !Guid.TryParse(profileIdClaim.Value, out var profileId))
        {
            logger.LogWarning("Permission check skipped: no valid profile_id claim present");

            return; // Do not call context.Succeed — fail authorization implicitly.
        }

        // Resolve the school from ITenantContext (populated by TenantMiddleware from X-School-Id).
        var tenantContext =
            httpContextAccessor.HttpContext?.RequestServices.GetService(typeof(ITenantContext))
            as ITenantContext;

        if (tenantContext is null || !tenantContext.IsResolved)
        {
            logger.LogWarning(
                "Permission check skipped: ITenantContext not resolved for profile {ProfileId}",
                profileId
            );

            return;
        }

        var schoolId = tenantContext.SchoolId;

        try
        {
            var reply = await grpcClient.CheckPermissionAsync(
                new CheckPermissionRequest
                {
                    UserId = profileId.ToString(),
                    SchoolId = schoolId.ToString(),
                    Permission = requirement.Permission,
                }
            );

            if (reply.HasPermission)
            {
                context.Succeed(requirement);
            }
        }
        catch (RpcException ex)
        {
            // Do not succeed on gRPC errors — fail authorization implicitly and log the error.
            logger.LogError(
                ex,
                "gRPC CheckPermission failed for profile {ProfileId}, school {SchoolId}, permission {Permission}",
                profileId,
                schoolId,
                requirement.Permission
            );
        }
    }
}
