using Edvantix.DataVault;

namespace Edvantix.EntityHub.Worker;

public static class ProjectAssembly
{
    public static Dictionary<string, System.Reflection.Assembly> Assemblies =>
        new() { { nameof(DataVault), typeof(IDataVaultApiMarker).Assembly } };
}
