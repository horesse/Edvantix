using Edvantix.Person.Features.ContactFeature;
using Edvantix.Person.Features.EmploymentHistoryFeature;
using Edvantix.Person.Features.FullNameFeature;
using Edvantix.Person.Features.PersonInfoFeature;

namespace Edvantix.Person.Features;

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
