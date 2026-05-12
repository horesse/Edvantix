using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Schedule.Extensions;

internal static class AuthorizationExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddSecurityServices()
        {
            var services = builder.Services;

            builder.AddDefaultAuthentication().WithKeycloakClaimsTransformation();

            services
                .AddAuthorizationBuilder()
                .SetDefaultPolicy(
                    new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .RequireScope(
                            $"{Services.Schedule}_{Authorization.Actions.Read}",
                            $"{Services.Schedule}_{Authorization.Actions.Write}"
                        )
                        .Build()
                );

            services.AddTransient(s =>
                s.GetRequiredService<IHttpContextAccessor>().HttpContext?.User
                ?? new ClaimsPrincipal()
            );

            services.AddKeycloakTokenIntrospection();
        }
    }
}
