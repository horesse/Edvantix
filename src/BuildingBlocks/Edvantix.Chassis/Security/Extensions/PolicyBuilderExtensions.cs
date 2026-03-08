using Edvantix.Chassis.Security.Authorization;
using Edvantix.Chassis.Security.Keycloak;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Security.Extensions;

public static class PolicyBuilderExtensions
{
    /// <summary>
    /// Добавляет требование наличия claim <c>profile_id</c> в токене.
    /// Используйте <see cref="AddProfileRequiredServices"/> для регистрации обработчиков.
    /// </summary>
    public static AuthorizationPolicyBuilder RequireProfileRegistered(
        this AuthorizationPolicyBuilder builder
    ) => builder.AddRequirements(new ProfileRegisteredRequirement());

    /// <summary>
    /// Регистрирует обработчик требования <see cref="ProfileRegisteredRequirement"/>
    /// и кастомный <see cref="IAuthorizationMiddlewareResultHandler"/>,
    /// который возвращает <c>PROFILE_NOT_REGISTERED</c> при отсутствии профиля.
    /// </summary>
    public static IServiceCollection AddProfileRequiredServices(
        this IServiceCollection services
    )
    {
        services.AddSingleton<IAuthorizationHandler, ProfileRegisteredRequirementHandler>();
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, ProfileAuthorizationResultHandler>();
        return services;
    }

    public static AuthorizationPolicyBuilder RequireScope(
        this AuthorizationPolicyBuilder authorizationPolicyBuilder,
        params string[] allowedValues
    )
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
