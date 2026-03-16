using System.Runtime.CompilerServices;

namespace Edvantix.Persona.ContractTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyMassTransit.Initialize();
    }
}
