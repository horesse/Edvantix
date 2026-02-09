using Edvantix.Chassis.Security.Settings;
using Edvantix.Chassis.Utilities;
using Edvantix.Chassis.Utilities.Configuration;
using Edvantix.Constants.Aspire;
using Edvantix.Constants.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edvantix.Chassis.Security.Extensions;

public static class AuthenticationExtensions
{
    public static IHostApplicationBuilder AddDefaultAuthentication(
        this IHostApplicationBuilder builder
    )
    {
        var services = builder.Services;

        services.Configure<IdentityOptions>(IdentityOptions.ConfigurationSection);

        var realm = services.BuildServiceProvider().GetRequiredService<IdentityOptions>().Realm;
        var keycloakUrlBuilt = HttpUtilities
            .AsUrlBuilder()
            .WithScheme(Http.Schemes.HttpOrHttps)
            .WithHost(Components.KeyCloak)
            .Build();

        // TODO: Фронт локально запускается в http, из-за этого отличается issuer и токен не проходит валидацию
        var keycloakUrl = builder.Configuration["KEYCLOAK_URL"] ?? keycloakUrlBuilt;

        services.AddHttpClient(
            Components.KeyCloak,
            client => client.BaseAddress = new(keycloakUrl)
        );

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddKeycloakJwtBearer(
                Components.KeyCloak,
                realm,
                options =>
                {
                    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                    options.Audience = "account";

                    // TODO: In dev we start http keycloak for front
                    options.TokenValidationParameters.ValidateIssuer = false;
                }
            );

        return builder;
    }
}
