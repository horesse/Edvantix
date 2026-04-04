using Edvantix.Chassis.Security.Keycloak;

namespace Edvantix.Persona.UnitTests.Helpers;

internal static class ServiceProviderHelper
{
    public static ClaimsPrincipal CreateClaimsPrincipal(
        Guid accountId,
        string login = "testuser"
    ) =>
        new(
            new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, accountId.ToString()),
                    new Claim(KeycloakClaimTypes.PreferredUsername, login),
                    new Claim(KeycloakClaimTypes.Profile, accountId.ToString()),
                ],
                "Bearer"
            )
        );
}
