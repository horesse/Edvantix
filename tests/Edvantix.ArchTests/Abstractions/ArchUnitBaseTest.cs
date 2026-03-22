using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;

namespace Edvantix.ArchTests.Abstractions;

public abstract class ArchUnitBaseTest : BaseTest
{
    protected static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            PersonaAssembly,
            NotificationAssembly,
            SchedulerAssembly,
            ChassisAssembly,
            ConstantsAssembly,
            SharedKernelAssembly
        )
        .Build();

    protected static readonly IObjectProvider<IType> PersonaServiceTypes = ArchRuleDefinition
        .Types()
        .That()
        .ResideInAssembly(PersonaAssembly)
        .And()
        .DoNotResideInNamespaceMatching("Microsoft.CodeCoverage.*")
        .As(nameof(Persona));

    protected static readonly IObjectProvider<IType> NotificationServiceTypes = ArchRuleDefinition
        .Types()
        .That()
        .ResideInAssembly(NotificationAssembly)
        .And()
        .DoNotResideInNamespaceMatching("Microsoft.CodeCoverage.*")
        .As(nameof(Notification));

    protected static readonly IObjectProvider<IType> SchedulerServiceTypes = ArchRuleDefinition
        .Types()
        .That()
        .ResideInAssembly(SchedulerAssembly)
        .And()
        .DoNotResideInNamespaceMatching("Microsoft.CodeCoverage.*")
        .As(nameof(Scheduler));

    protected static readonly IObjectProvider<IType> ChassisServiceTypes = ArchRuleDefinition
        .Types()
        .That()
        .ResideInAssembly(ChassisAssembly)
        .And()
        .DoNotResideInNamespaceMatching("Microsoft.CodeCoverage.*")
        .As(nameof(Chassis));

    protected static readonly IObjectProvider<IType> ConstantsServiceTypes = ArchRuleDefinition
        .Types()
        .That()
        .ResideInAssembly(ConstantsAssembly)
        .And()
        .DoNotResideInNamespaceMatching("Microsoft.CodeCoverage.*")
        .As(nameof(Constants));

    protected static readonly IObjectProvider<IType> SharedKernelServiceTypes = ArchRuleDefinition
        .Types()
        .That()
        .ResideInAssembly(SharedKernelAssembly)
        .And()
        .DoNotResideInNamespaceMatching("Microsoft.CodeCoverage.*")
        .As(nameof(SharedKernel));

    protected static IObjectProvider<IType> GetServiceTypes(string serviceName)
    {
        return serviceName switch
        {
            nameof(Persona) => PersonaServiceTypes,
            nameof(Notification) => NotificationServiceTypes,
            nameof(Scheduler) => SchedulerServiceTypes,
            nameof(Chassis) => ChassisServiceTypes,
            nameof(Constants) => ConstantsServiceTypes,
            nameof(SharedKernel) => SharedKernelServiceTypes,
            _ => throw new ArgumentException($"Unknown service: {serviceName}"),
        };
    }
}
