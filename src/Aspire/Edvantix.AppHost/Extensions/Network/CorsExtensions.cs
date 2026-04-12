using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Edvantix.AppHost.Extensions.Network;

internal static class CorsExtensions
{
    private static readonly string[] _defaultHeaders =
    [
        HeaderNames.ContentType,
        HeaderNames.Authorization,
        HeaderNames.Accept,
        HeaderNames.Origin,
        HeaderNames.XRequestedWith,
        HeaderNames.XPoweredBy,
    ];

    private static readonly string[] _defaultMethods =
    [
        HttpMethods.Get,
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Delete,
        HttpMethods.Patch,
        HttpMethods.Options,
    ];

    extension(IDistributedApplicationBuilder builder)
    {
        public (
            IResourceBuilder<ParameterResource> OrganizationFrontUrl,
            IResourceBuilder<ParameterResource> AdminFrontUrl
        ) AddCorsOriginParameters()
        {
            var organizationFrontUrl = builder
                .AddParameter("organizationfront-url")
                .WithCustomInput(_ =>
                    new()
                    {
                        Name = "OrganizationFrontUrlParameter",
                        Label = "OrganizationFront URL",
                        InputType = InputType.Text,
                        Description =
                            "Enter the OrganizationFront application URL for CORS (e.g., https://org.edvantix.ru)",
                    }
                );

            var adminFrontUrl = builder
                .AddParameter("adminfront-url")
                .WithCustomInput(_ =>
                    new()
                    {
                        Name = "AdminFrontUrlParameter",
                        Label = "AdminFront URL",
                        InputType = InputType.Text,
                        Description =
                            "Enter the AdminFront application URL for CORS (e.g., https://admin.edvantix.ru)",
                    }
                );

            return (organizationFrontUrl, adminFrontUrl);
        }
    }

    extension(IResourceBuilder<ProjectResource> builder)
    {
        public IResourceBuilder<ProjectResource> WithCorsOrigins(
            IResourceBuilder<ParameterResource> organizationFrontUrl,
            IResourceBuilder<ParameterResource> adminFrontUrl
        )
        {
            builder
                .WithEnvironment("Cors__Origins__0", organizationFrontUrl)
                .WithEnvironment("Cors__Origins__1", adminFrontUrl)
                .WithEnvironment("Cors__AllowCredentials", "true");

            for (var i = 0; i < _defaultHeaders.Length; i++)
            {
                builder.WithEnvironment($"Cors__Headers__{i}", _defaultHeaders[i]);
            }

            for (var i = 0; i < _defaultMethods.Length; i++)
            {
                builder.WithEnvironment($"Cors__Methods__{i}", _defaultMethods[i]);
            }

            return builder;
        }
    }
}
