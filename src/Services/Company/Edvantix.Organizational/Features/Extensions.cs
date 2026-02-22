using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features;

public static class Extensions
{
    public static void AddApiFeature(this IServiceCollection services)
    {
        services.AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>();
    }
}
