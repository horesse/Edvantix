using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;
using Edvantix.Organizational.Features.OrganizationCustomRoleFeature;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features;

public static class Extensions
{
    public static void AddApiFeature(this IServiceCollection services)
    {
        services.AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>();
        services.AddScoped<IOrganizationCustomRoleService, OrganizationCustomRoleService>();

        // Матрица доступа — stateless singleton (использует frozen collections).
        services.AddSingleton<IOrganizationPermissionMatrix, OrganizationPermissionMatrix>();
        services.AddScoped<IOrganizationPermissionService, OrganizationPermissionService>();
    }
}
