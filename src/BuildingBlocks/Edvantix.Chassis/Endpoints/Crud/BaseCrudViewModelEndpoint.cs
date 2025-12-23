using Asp.Versioning;
using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Chassis.Utilities.Formatters;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.SeedWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Edvantix.Chassis.Endpoints.Crud;

/// <summary>
/// Базовый класс для CRUD endpoints с поддержкой ViewModels
/// </summary>
public abstract class BaseCrudViewModelEndpoint<TModel, TIdentity>
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    protected virtual string ResourceName => typeof(TModel).Name.Replace("Model", string.Empty);

    protected virtual ApiVersion ApiVersion => new(1, 0);
    protected virtual bool RequireAuth => typeof(TModel).IsAuthRequired();
    protected virtual string? AuthPolicy => RequireAuth ? typeof(TModel).GetPolicy() : null;
    protected virtual string Description => typeof(TModel).GetDescription();
    protected virtual string ResourceDisplayName => $"{Description} ({ResourceName})";
    protected virtual string Tag => ResourceDisplayName;

    protected RouteHandlerBuilder ConfigureEndpoint(
        RouteHandlerBuilder builder,
        string name,
        string summary,
        string? description = null
    )
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
            builder =
                AuthPolicy != null
                    ? builder.RequireAuthorization(AuthPolicy)
                    : builder.RequireAuthorization();
        }

        return builder;
    }

    protected virtual string GetRoutePath(CrudAction action, bool identity = false)
    {
        var route = $"/{ResourceName.ToSnakeCase()}/{action.ToString().ToSnakeCase()}";

        if (identity)
            route += "/{id}";

        return route;
    }
}
