using ArchUnitNET.TUnit;
using Edvantix.ArchTests.Abstractions;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Edvantix.ArchTests.Domain;

public sealed class CrossDomainDependencyTests : ArchUnitBaseTest
{
    public void GivenDomains_WhenCheckingDependencies_ThenShouldNotDependOnOtherDomains(
        string sourceDomain,
        string targetDomain
    )
    {
        Types()
            .That()
            .ResideInNamespaceMatching($"{nameof(Edvantix)}.{sourceDomain}.Domain")
            .Should()
            .NotDependOnAny(
                Types()
                    .That()
                    .ResideInNamespaceMatching($"{nameof(Edvantix)}.{targetDomain}.Domain")
            )
            .Because(
                $"Domain {sourceDomain} should not depend on domain {targetDomain} to maintain proper bounded context separation."
            )
            .Check(Architecture);
    }
}
