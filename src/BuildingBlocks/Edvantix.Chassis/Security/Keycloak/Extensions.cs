using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edvantix.Chassis.Security.Keycloak;

public static class KeycloakClaimsTransformationExtensions
{
    /// <summary>
    ///     Adds an <see cref="IClaimsTransformation" /> that transforms Keycloak resource access roles claims into regular
    ///     role claims.
    /// </summary>
    public static IHostApplicationBuilder WithKeycloakClaimsTransformation(
        this IHostApplicationBuilder builder
    )
    {
        builder.Services.AddTransient<IClaimsTransformation, KeycloakRolesClaimsTransformation>();
        return builder;
    }

    public static IServiceCollection AddKeycloakTokenIntrospection(this IServiceCollection services)
    {
        return services.AddScoped<KeycloakTokenIntrospectionMiddleware>();
    }

    public static IApplicationBuilder UseKeycloakTokenIntrospection(this IApplicationBuilder app)
    {
        return app.UseMiddleware<KeycloakTokenIntrospectionMiddleware>();
    }
}
