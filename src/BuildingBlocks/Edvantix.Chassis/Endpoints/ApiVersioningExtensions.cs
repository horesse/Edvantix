using Asp.Versioning;
using Edvantix.Constants.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Endpoints;

public static class Extension
{
    public static void AddVersioning(this IServiceCollection service)
    {
        service
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = ApiVersions.V1;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });
    }
}
