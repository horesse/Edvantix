using ArchUnitNET.TUnit;
using Edvantix.ArchTests.Abstractions;
using Mediator;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Edvantix.ArchTests.Domain;

public sealed class DomainEventHandlerTests : ArchUnitBaseTest
{
    [Test]
    public void GivenDomainEventHandlers_WhenCheckingModifiers_ThenShouldBeSealed()
    {
        Classes()
            .That()
            .HaveNameEndingWith("Handler")
            .And()
            .ResideInNamespaceMatching($"{nameof(Edvantix)}.*.Domain.EventHandlers")
            .Should()
            .BeSealed()
            .Because(
                "Domain event handlers should be sealed to prevent inheritance and ensure predictable behavior."
            )
            .Check(Architecture);
    }

    [Test]
    public void GivenDomainEventHandlers_WhenCheckingInterface_ThenShouldImplementINotificationHandler()
    {
        Classes()
            .That()
            .HaveNameEndingWith("Handler")
            .And()
            .ResideInNamespaceMatching($"{nameof(Edvantix)}.*.Domain.EventHandlers")
            .Should()
            .ImplementInterface(typeof(INotificationHandler<>))
            .Because(
                "Domain event handlers should implement INotificationHandler<T> from Mediator to handle domain events."
            )
            .Check(Architecture);
    }

    [Test]
    public void GivenDomainEventHandlers_WhenCheckingDependencies_ThenShouldNotDependOnFeatures()
    {
        Classes()
            .That()
            .HaveNameEndingWith("Handler")
            .And()
            .ResideInNamespaceMatching($"{nameof(Edvantix)}.*.Domain.EventHandlers")
            .Should()
            .NotDependOnAny(
                Types().That().ResideInNamespaceMatching($"{nameof(Edvantix)}.*.Features.*")
            )
            .Because(
                "Domain event handlers should not depend on Features to maintain proper layer separation."
            )
            .Check(Architecture);
    }
}
