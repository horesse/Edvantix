using Microsoft.AspNetCore.Authentication.OAuth;
using Scalar.Aspire;

namespace Edvantix.AppHost.Extensions.Infrastructure;

internal static class ScalarExtensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        /// <summary>
        /// Добавляет Scalar API reference к построителю распределённого приложения с предустановленными параметрами темы и шрифтов.
        /// Поддерживается только локальная аутентификация через Keycloak.
        /// </summary>
        /// <param name="keycloak">Построитель ресурса Keycloak для аутентификации.</param>
        /// <returns><see cref="IResourceBuilder{ScalarResource}" />, настроенный с заданными параметрами темы и шрифтов.</returns>
        public IResourceBuilder<ScalarResource> AddScalar(
            IResourceBuilder<IResource>? keycloak = null
        )
        {
            var scalar = builder.AddScalarApiReference(options =>
                options.DisableDefaultFonts().PreferHttpsEndpoint().AllowSelfSignedCertificates()
            );

            if (keycloak is null)
            {
                return scalar;
            }

            return keycloak switch
            {
                IResourceBuilder<KeycloakResource> container => scalar
                    .WithReference(container)
                    .WaitForStart(container),

                _ => throw new InvalidOperationException(
                    "Unsupported Keycloak resource builder type."
                ),
            };
        }
    }

    extension(IResourceBuilder<ScalarResource> builder)
    {
        /// <summary>
        /// Настраивает построитель ресурса Scalar для включения API reference с авторизацией OAuth.
        /// </summary>
        /// <param name="api">Построитель ресурса проекта, представляющий API проект.</param>
        /// <returns>Настроенный построитель ресурса Scalar с авторизацией OAuth.</returns>
        /// <exception cref="InvalidOperationException">
        /// Выбрасывается, когда ресурс Keycloak не найден в построителе приложения или обязательный параметр 'kc-realm' не настроен.
        /// </exception>
        public IResourceBuilder<ScalarResource> WithOpenAPI(IResourceBuilder<ProjectResource> api)
        {
            return builder.WithApiReference(
                api,
                async (options, ctx) =>
                {
                    var clientId = api.Resource.Name;

                    var parameter = builder
                        .ApplicationBuilder.Resources.OfType<ParameterResource>()
                        .FirstOrDefault(r =>
                            string.Equals(
                                r.Name,
                                $"{clientId}-secret",
                                StringComparison.OrdinalIgnoreCase
                            )
                        );

                    if (parameter is not null)
                    {
                        var clientSecret = await parameter.GetValueAsync(ctx);

                        options
                            .AddPreferredSecuritySchemes(OAuthDefaults.DisplayName)
                            .AddAuthorizationCodeFlow(
                                OAuthDefaults.DisplayName,
                                flow =>
                                {
                                    flow.WithPkce(Pkce.Sha256)
                                        .WithClientId(clientId)
                                        .AddBodyParameter("audience", "account");

                                    if (!string.IsNullOrWhiteSpace(clientSecret))
                                    {
                                        flow.WithClientSecret(clientSecret);
                                    }

                                    flow.WithSelectedScopes(
                                        $"{clientId}_{Authorization.Actions.Read}",
                                        $"{clientId}_{Authorization.Actions.Write}"
                                    );
                                }
                            );
                    }
                }
            );
        }
    }
}
