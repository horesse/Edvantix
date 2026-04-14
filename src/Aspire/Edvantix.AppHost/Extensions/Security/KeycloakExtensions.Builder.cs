namespace Edvantix.AppHost.Extensions.Security;

internal static partial class KeycloakExtensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        /// <summary>
        /// Добавляет ресурс контейнера Keycloak в построитель распределённого приложения
        /// с пользовательской темой и настройками импорта realm.
        /// </summary>
        /// <param name="name">Имя ресурса Keycloak.</param>
        /// <returns>
        /// <see cref="IResourceBuilder{KeycloakResource}" /> с настроенным ресурсом Keycloak.
        /// </returns>
        public IResourceBuilder<KeycloakResource> AddLocalKeycloak(string name)
        {
            var keycloak = builder
                .AddKeycloak(name)
                .WithDataVolume()
                .WithOtlpExporter()
                .WithArgs("--health-enabled=true")
                .WithIconName("LockClosedRibbon")
                .WithCustomTheme(_defaultLocalKeycloakName)
                .WithImagePullPolicy(ImagePullPolicy.Always)
                .WithLifetime(ContainerLifetime.Persistent)
                .WithSampleRealmImport(_defaultLocalKeycloakName, nameof(Edvantix));

            return keycloak;
        }

        /// <summary>
        /// Добавляет размещённый внешний сервис Keycloak в построитель распределённого приложения.
        /// </summary>
        /// <param name="name">Имя ресурса внешнего сервиса Keycloak.</param>
        /// <returns>
        /// <see cref="IResourceBuilder{ExternalServiceResource}" /> с настроенным внешним сервисом Keycloak.
        /// </returns>
        public IResourceBuilder<ExternalServiceResource> AddHostedKeycloak(string name)
        {
            var keycloakUrl = builder
                .AddParameter("kc-url", true)
                .WithCustomInput(_ =>
                    new()
                    {
                        Name = "KeycloakUrlParameter",
                        Label = "Keycloak URL",
                        InputType = InputType.Text,
                        Value = "https://identity.bookworm.com",
                        Description = "Enter your Keycloak server URL here (must be https)",
                    }
                );

            builder
                .AddParameter("kc-realm", true)
                .WithCustomInput(_ =>
                    new()
                    {
                        Name = "KeycloakRealmParameter",
                        Label = "Keycloak Realm",
                        InputType = InputType.Text,
                        Value = nameof(Edvantix).ToLowerInvariant(),
                        Description = "Enter your Keycloak realm name here",
                    }
                );

            var keycloak = builder
                .AddExternalService(name, keycloakUrl)
                .WithIconName("LockClosedRibbon")
                .WithHttpHealthCheck("/health/ready");

            keycloakUrl.WithParentRelationship(keycloak);

            return keycloak;
        }
    }

    extension(IResourceBuilder<KeycloakResource> builder)
    {
        private IResourceBuilder<KeycloakResource> WithSampleRealmImport(
            string realmName,
            string displayName
        )
        {
            builder
                .WithRealmImport($"{BaseContainerPath}/realms")
                .WithEnvironment(RealmName, realmName)
                .WithEnvironment(RealmDisplayName, displayName);

            return builder;
        }

        private IResourceBuilder<KeycloakResource> WithCustomTheme(string themeName)
        {
            var importFullPath = Path.GetFullPath(
                $"{BaseContainerPath}/themes",
                builder.ApplicationBuilder.AppHostDirectory
            );

            if (Directory.Exists(importFullPath))
            {
                builder
                    .WithBindMount(importFullPath, "/opt/keycloak/providers/", true)
                    .WithEnvironment(ThemeName, themeName);
            }

            return builder;
        }
    }
}
