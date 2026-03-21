using Edvantix.Organizations.Features.Permissions.CheckPermission;
using Edvantix.Organizations.Grpc.Generated;
using Grpc.Core;
using Mediator;

namespace Edvantix.Organizations.Grpc.Services;

/// <summary>
/// gRPC server implementation of the <c>PermissionsGrpcService</c> contract.
/// Delegates permission checks to the application layer via Mediator,
/// which uses <see cref="IHybridCache"/> to serve cached results.
/// </summary>
public sealed class PermissionsGrpcService(IMediator mediator)
    : Generated.PermissionsGrpcService.PermissionsGrpcServiceBase
{
    /// <summary>
    /// Checks whether the specified user holds the requested permission within a school.
    /// </summary>
    /// <param name="request">Contains <c>UserId</c>, <c>SchoolId</c>, and the <c>Permission</c> string.</param>
    /// <param name="context">The server call context (unused — gRPC infrastructure).</param>
    /// <returns>A reply with <c>HasPermission</c> set to <c>true</c> if the permission is granted.</returns>
    public override async Task<CheckPermissionReply> CheckPermission(
        CheckPermissionRequest request,
        ServerCallContext context
    )
    {
        var userId = Guid.Parse(request.UserId);
        var schoolId = Guid.Parse(request.SchoolId);

        var result = await mediator.Send(
            new GetUserPermissionGrantQuery(userId, schoolId, request.Permission),
            context?.CancellationToken ?? CancellationToken.None
        );

        return new CheckPermissionReply { HasPermission = result };
    }
}
