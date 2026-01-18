using System.Reflection;
using Edvantix.Company;
using Edvantix.DataVault;
using Edvantix.ProfileService;

namespace Edvantix.EntityHub.Worker;

public static class ProjectAssembly
{
    public static Dictionary<string, Assembly> Assemblies =>
        new()
        {
            { nameof(DataVault), typeof(IDataVaultApiMarker).Assembly },
            { nameof(ProfileService), typeof(IProfileApiMarker).Assembly },
            { nameof(Company), typeof(IOrganizationApiMarker).Assembly },
        };
}
