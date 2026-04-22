using System.Runtime.CompilerServices;

namespace Edvantix.Notification.UnitTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifySendGrid.Initialize();

        DerivePathInfo(
            (_, projectDirectory, type, method) =>
                new(Path.Combine(projectDirectory, "Snapshots"), type.Name, method.Name)
        );
    }
}
