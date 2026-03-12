using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Persona.Extensions;

internal static class AuthorizationExtensions
{
    public static void AddSecurityServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        
        builder.AddDefaultAuthentication().WithKeycloakClaimsTransformation();
        
        services
            .AddAuthorizationBuilder()
            .AddPolicy(
                Authorization.Policies.Admin,
                policy =>
                {
                    policy
                        .RequireAuthenticatedUser()
                        .RequireRole(Authorization.Roles.Admin)
                        .RequireScope(
                            $"{Services.Persona}_{Authorization.Actions.Read}",
                            $"{Services.Persona}_{Authorization.Actions.Write}"
                        );
                }
            )
            .SetDefaultPolicy(
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireScope($"{Services.Persona}_{Authorization.Actions.Read}")
                    .Build()
            );
        
        services.AddTransient(s =>
            s.GetRequiredService<IHttpContextAccessor>().HttpContext?.User ?? new ClaimsPrincipal()
        );
        
        services.AddKeycloakTokenIntrospection();
    }
}
