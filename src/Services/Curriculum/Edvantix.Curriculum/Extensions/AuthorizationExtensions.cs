using Edvantix.Chassis.Security.Extensions;
using Edvantix.Chassis.Security.Keycloak;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Curriculum.Extensions;

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
                            $"{Services.Curriculum}_{Authorization.Actions.Read}",
                            $"{Services.Curriculum}_{Authorization.Actions.Write}"
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
