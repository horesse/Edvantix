using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Chassis.Security.Settings;
using Edvantix.Chassis.Utilities;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;

internal sealed class SecuritySchemeDefinitionsTransformer(IdentityOptions identityOptions)
    : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        var keycloakUrl = ServiceDiscoveryUtilities.GetServiceEndpoint(
            Components.KeyCloak,
            0,
            true
        );

        if (string.IsNullOrWhiteSpace(keycloakUrl))
        {
            return Task.CompletedTask;
        }

        var authUrl = HttpUtilities
            .AsUrlBuilder()
            .WithBase(keycloakUrl)
            .WithPath(KeycloakEndpoints.Authorize(identityOptions.Realm))
            .Build();

        var tokenUrl = HttpUtilities
            .AsUrlBuilder()
            .WithScheme(Http.Schemes.Http)
            .WithHost(Components.KeyCloak)
            .WithPath(KeycloakEndpoints.Token(identityOptions.Realm))
            .Build();

        var securityScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Description = "OAuth2 security scheme for the Edvantix API",
            Flows = new()
            {
                AuthorizationCode = new()
                {
                    Scopes = identityOptions.Scopes!,
                    AuthorizationUrl = new(authUrl),
                    TokenUrl = new(tokenUrl),
                },
            },
        };

        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes.Add(OAuthDefaults.DisplayName, securityScheme);

        return Task.CompletedTask;
    }
}
