using Edvantix.Company.Services;

namespace Edvantix.Company.Features;

public static class Extensions
{
    public static void AddApiFeature(this IServiceCollection services)
    {
        services.AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>();
    }
}
