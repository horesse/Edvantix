using Edvantix.ProfileService.Features.EmploymentHistoryFeature;
using Edvantix.ProfileService.Features.FullNameFeature;
using Edvantix.ProfileService.Features.ProfileFeature;
using Edvantix.ProfileService.Features.UserContactFeature;

namespace Edvantix.ProfileService.Features;

public static class Extensions
{
    public static void AddApiFeature(this IServiceCollection services)
    {
        services.AddContactFeature();
        services.AddEmploymentHistoryFeature();
        services.AddFullNameFeature();
        services.AddPersonInfoFeature();
    }
}
