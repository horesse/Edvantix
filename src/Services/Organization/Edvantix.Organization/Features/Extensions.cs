using Edvantix.Organization.Features.Contact;
using Edvantix.Organization.Features.Member;
using Edvantix.Organization.Features.Org;
using Edvantix.Organization.Features.Usage;

namespace Edvantix.Organization.Features;

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
