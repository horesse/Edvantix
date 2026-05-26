namespace Edvantix.AppHost.Extensions.Security;

internal static partial class KeycloakExtensions
{
    extension(IResourceBuilder<TurborepoAppResource> builder)
    {
        /// <summary>
        /// Настраивает ресурс приложения Turborepo для интеграции с Keycloak как поставщиком удостоверений (IdP).
        /// </summary>
        /// <param name="keycloak">Построитель ресурса Keycloak, настраиваемый как IdP.</param>
        /// <returns>Построитель ресурса приложения Turborepo для цепочки вызовов.</returns>
        public IResourceBuilder<TurborepoAppResource> WithKeycloak(
            IResourceBuilder<IResource> keycloak
        )
        {
            var clientId = builder.Resource.Name;

            switch (keycloak)
            {
                case IResourceBuilder<KeycloakResource> keycloakContainer:
                    ConfigureKeycloakForClient(
                        keycloakContainer,
                        builder,
                        clientId,
                        "APP",
                        null,
                        false
                    );

                    // NEXT_PUBLIC_* so the browser-side react-oidc-context can reach Keycloak.
                    // The endpoint URL is the Aspire-proxied host port — visible to the browser,
                    // unlike the internal Docker network address (http://keycloak:8080).
                    builder
                        .WithReference(keycloakContainer)
                        .WaitForStart(keycloakContainer)
                        .WithEnvironment(
                            "NEXT_PUBLIC_KEYCLOAK_URL",
                            keycloakContainer.GetEndpoint(Uri.UriSchemeHttp)
                        )
                        .WithEnvironment("NEXT_PUBLIC_KEYCLOAK_REALM", _defaultLocalKeycloakName)
                        .WithEnvironment("NEXT_PUBLIC_KEYCLOAK_CLIENT_ID", clientId);
                    break;

                case IResourceBuilder<ExternalServiceResource> keycloakHosted:
                    ConfigureClientForHostedKeycloak(builder, keycloakHosted, clientId);
                    break;
            }

            return builder;
        }
    }
}
