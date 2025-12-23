using Edvantix.Person.Features.ContactFeature;

namespace Edvantix.Person.Features;

public static class Extensions
{
    public static void AddApiFeature(this IServiceCollection services)
    {
        services.AddContactFeature();
    }
}
