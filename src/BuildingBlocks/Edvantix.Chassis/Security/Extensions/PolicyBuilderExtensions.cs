using Edvantix.Chassis.Security.Keycloak;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Chassis.Security.Extensions;

public static class PolicyBuilderExtensions
{
    extension(AuthorizationPolicyBuilder authorizationPolicyBuilder)
    {
        /// <summary>
        /// Требует, чтобы пользователь содержал хотя бы одно допустимое значение scope в Keycloak-клейме scope.
        /// </summary>
        /// <param name="allowedValues">
        /// Допустимые значения scope.
        /// </param>
        /// <returns>
        /// Настроенный построитель политики авторизации.
        /// </returns>
        public AuthorizationPolicyBuilder RequireScope(params string[] allowedValues)
        {
            var scopeClaim = authorizationPolicyBuilder.RequireAssertion(context =>
            {
                var scopeClaim = context.User.FindFirst(KeycloakClaimTypes.Scope);

                if (scopeClaim is null)
                {
                    return false;
                }

                var scopes = scopeClaim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return scopes.Any(s => allowedValues.Contains(s, StringComparer.OrdinalIgnoreCase));
            });

            return scopeClaim;
        }
    }
}
