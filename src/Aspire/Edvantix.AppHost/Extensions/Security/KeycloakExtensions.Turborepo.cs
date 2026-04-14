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

            var betterAuthSecret = builder
                .ApplicationBuilder.AddParameter($"{clientId}-better-auth-secret", true)
                .WithGeneratedDefault(new() { MinLength = 32, Special = false });

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

                    builder
                        .WithReference(keycloakContainer)
                        .WaitForStart(keycloakContainer)
                        .WithEnvironment("BETTER_AUTH_SECRET", betterAuthSecret)
                        .WithEnvironment("KEYCLOAK_REALM", _defaultLocalKeycloakName)
                        .WithEnvironment("KEYCLOAK_CLIENT_ID", clientId);
                    break;
                case IResourceBuilder<ExternalServiceResource> keycloakHosted:
                    ConfigureClientForHostedKeycloak(
                        builder,
                        keycloakHosted,
                        betterAuthSecret,
                        clientId
                    );
                    break;
            }

            return builder;
        }
    }
}
