using ArchUnitNET.TUnit;
using Edvantix.ArchTests.Abstractions;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Edvantix.ArchTests.Dependencies;

public sealed class CyclicDependencyTests : ArchUnitBaseTest
{
    private static readonly string[] ServiceNames =
    [
        nameof(Blog),
        nameof(Catalog),
        nameof(Organizational),
        nameof(Persona),
        nameof(Notification),
        nameof(Scheduler),
        nameof(Subscriptions),
    ];

    [Test]
    [Arguments(nameof(Blog))]
    [Arguments(nameof(Catalog))]
    [Arguments(nameof(Organizational))]
    [Arguments(nameof(Persona))]
    [Arguments(nameof(Notification))]
    [Arguments(nameof(Scheduler))]
    [Arguments(nameof(Subscriptions))]
    public void GivenService_WhenCheckingDependencies_ThenShouldNotDependOnOtherServices(
        string serviceName
    )
    {
        var serviceTypes = GetServiceTypes(serviceName);
        var otherServices = ServiceNames.Where(s => s != serviceName).ToArray();

        var rule = Types()
            .That()
            .Are(serviceTypes)
            .Should()
            .NotDependOnAnyTypesThat()
            .Are(GetServiceTypes(otherServices[0]));

        for (var i = 1; i < otherServices.Length; i++)
        {
            rule = rule.OrShould().NotDependOnAnyTypesThat().Are(GetServiceTypes(otherServices[i]));
        }

        rule.Because(
                $"{serviceName} service should be independent and not directly depend on other services."
            )
            .Check(Architecture);
    }

    [Test]
    [Arguments(nameof(Chassis))]
    [Arguments(nameof(Constants))]
    [Arguments(nameof(SharedKernel))]
    public void GivenBuildingBlock_WhenCheckingDependencies_ThenShouldNotDependOnServices(
        string buildingBlockName
    )
    {
        var buildingBlockTypes = GetServiceTypes(buildingBlockName);

        var rule = Types()
            .That()
            .Are(buildingBlockTypes)
            .Should()
            .NotDependOnAnyTypesThat()
            .Are(GetServiceTypes(ServiceNames[0]));

        for (var i = 1; i < ServiceNames.Length; i++)
        {
            rule = rule.OrShould().NotDependOnAnyTypesThat().Are(GetServiceTypes(ServiceNames[i]));
        }

        rule.Because(
                $"{buildingBlockName} should be independent and not depend on specific services."
            )
            .Check(Architecture);
    }
}
