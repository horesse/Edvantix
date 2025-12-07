using Asp.Versioning.ApiExplorer;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi.Transformers;
using Microsoft.AspNetCore.OpenApi;

namespace Edvantix.ServiceDefaults.ApiSpecification.OpenApi;

public static class OpenApiOptionsExtensions
{
    extension(OpenApiOptions options)
    {
        public void ApplyApiVersionInfo(
            DocumentOptions? openApiDocument,
            ApiVersionDescription apiDescription
        )
        {
            options.AddDocumentTransformer(
                new OpenApiInfoDefinitionsTransformer(openApiDocument, apiDescription)
            );
        }

        public void ApplySecuritySchemeDefinitions()
        {
            options.AddDocumentTransformer<SecuritySchemeDefinitionsTransformer>();
        }

        public void ApplyOperationDeprecatedStatus()
        {
            options.AddOperationTransformer<OperationDeprecatedStatusTransformer>();
        }

        public void ApplyAuthorizationChecks(string[] scopes)
        {
            options.AddOperationTransformer(new AuthorizationChecksTransformer(scopes));
        }
    }
}
