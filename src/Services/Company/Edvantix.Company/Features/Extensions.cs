using Edvantix.Company.Features.Contact;
using Edvantix.Company.Features.Member;
using Edvantix.Company.Features.Org;
using Edvantix.Company.Features.Usage;

namespace Edvantix.Company.Features;

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
