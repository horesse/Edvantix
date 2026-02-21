using System.Reflection;
using Edvantix.Company;
using Edvantix.ProfileService;

namespace Edvantix.EntityHub.Worker;

public static class ProjectAssembly
{
    public static Dictionary<string, Assembly> Assemblies =>
        new()
        {
            { nameof(ProfileService), typeof(IProfileApiMarker).Assembly },
            { nameof(Company), typeof(IOrganizationApiMarker).Assembly },
        };
}
