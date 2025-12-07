using Asp.Versioning;
using Edvantix.SharedKernel.SeedWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Edvantix.Chassis.Endpoints.Crud;

/// <summary>
/// Базовый класс для CRUD endpoints с общей конфигурацией
/// </summary>
public abstract class BaseCrudEndpoint<TModel, TIdentity>
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    protected abstract string ResourceName { get; }
    protected abstract string Tag { get; }
    
    protected virtual string GetRoutePath(string action) => $"/{ResourceName}/{action}";
    protected virtual ApiVersion ApiVersion => new(1, 0);
    protected virtual bool RequireAuth => false;
    protected virtual string? AuthPolicy => null;

    protected RouteHandlerBuilder ConfigureEndpoint(
        RouteHandlerBuilder builder,
        string name,
        string summary,
        string? description = null)
    {
        builder = builder
            .WithTags(Tag)
            .WithName(name)
            .WithSummary(summary)
            .MapToApiVersion(ApiVersion);

        if (!string.IsNullOrEmpty(description))
        {
            builder = builder.WithDescription(description);
        }

        if (RequireAuth)
        {
            builder = AuthPolicy != null
                ? builder.RequireAuthorization(AuthPolicy)
                : builder.RequireAuthorization();
        }

        return builder;
    }
}
