using Edvantix.OrganizationManagement.Features.Contact;
using Edvantix.OrganizationManagement.Features.Member;
using Edvantix.OrganizationManagement.Features.Organization;
using Edvantix.OrganizationManagement.Features.Usage;

namespace Edvantix.OrganizationManagement.Features;

public static class Extensions
{
    public static void AddApiFeature(this IServiceCollection services)
    {
        services.AddContactFeature();
        services.AddMemberFeature();
        services.AddOrganizationFeature();
        services.AddUsageFeature();
    }
}

