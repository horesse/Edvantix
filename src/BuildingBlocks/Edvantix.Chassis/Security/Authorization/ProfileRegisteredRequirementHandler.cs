using Edvantix.Chassis.Security.Keycloak;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Chassis.Security.Authorization;

/// <summary>
/// Проверяет наличие claim <c>profile_id</c> в токене пользователя.
/// Если claim отсутствует или невалиден, завершает авторизацию с
/// <see cref="ProfileNotRegisteredFailureReason"/>.
/// </summary>
internal sealed class ProfileRegisteredRequirementHandler
    : AuthorizationHandler<ProfileRegisteredRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProfileRegisteredRequirement requirement
    )
    {
        var profileIdClaim = context.User.FindFirst(KeycloakClaimTypes.ProfileId);

        if (profileIdClaim is not null && Guid.TryParse(profileIdClaim.Value, out _))
        {
            context.Succeed(requirement);
        }
        else
        {
            // Передаём this как IAuthorizationHandler — требование AuthorizationFailureReason.
            context.Fail(new ProfileNotRegisteredFailureReason(this));
        }

        return Task.CompletedTask;
    }
}
