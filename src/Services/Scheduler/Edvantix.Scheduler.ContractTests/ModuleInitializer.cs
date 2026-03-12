using System.Runtime.CompilerServices;

namespace Edvantix.Scheduler.ContractTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyWolverine.Initialize();
    }
}
