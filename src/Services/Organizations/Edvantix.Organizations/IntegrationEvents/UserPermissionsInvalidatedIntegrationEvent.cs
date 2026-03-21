namespace Edvantix.Contracts;

/// <summary>
/// Published when a user's effective permissions may have changed due to a role assignment,
/// role revocation, or a role's permission set being updated.
/// <para>
/// <see cref="UserId"/> is null when a role's permissions changed — this affects all users
/// holding that role in the school. Consumers should invalidate by school-scoped tag when
/// <see cref="UserId"/> is null.
/// </para>
/// </summary>
public sealed record UserPermissionsInvalidatedIntegrationEvent(
    Guid? UserId,
    Guid SchoolId,
    DateTimeOffset Timestamp
) : IntegrationEvent;
