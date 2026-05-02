using ArchUnitNET.TUnit;
using Edvantix.ArchTests.Abstractions;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Edvantix.ArchTests.Features;

/// <summary>
/// Enforces Wolverine-specific architectural conventions for integration event handlers.
/// Wolverine discovers handlers by naming conventions; violations cause silent runtime failures.
/// </summary>
public sealed class WolverineHandlerConventionTests : ArchUnitBaseTest
{
    private const string IntegrationEventHandlerNamespace =
        $"{nameof(Edvantix)}.*.IntegrationEvents.EventHandlers";

    [Test]
    public void GivenIntegrationEventHandlers_WhenCheckingVisibility_ThenShouldBePublic()
    {
        Classes()
            .That()
            .ResideInNamespaceMatching(IntegrationEventHandlerNamespace)
            .And()
            .HaveNameEndingWith("Handler")
            .Should()
            .BePublic()
            .Because(
                "Wolverine only discovers public handler types. A non-public handler is silently skipped during startup scanning."
            )
            .Check(Architecture);
    }

    [Test]
    public void GivenIntegrationEventHandlers_WhenCheckingDependencies_ThenShouldNotUseServiceLocator()
    {
        Classes()
            .That()
            .ResideInNamespaceMatching(IntegrationEventHandlerNamespace)
            .And()
            .HaveNameEndingWith("Handler")
            .Should()
            .FollowCustomCondition(
                cls =>
                    !cls.Dependencies.Select(d => d.Target).Any(t => t.Name == "IServiceProvider"),
                "should not depend on IServiceProvider",
                "Wolverine handlers receive dependencies via method injection. IServiceProvider indicates the service locator anti-pattern."
            )
            .Check(Architecture);
    }

    [Test]
    public void GivenIntegrationEventHandlers_WhenCheckingDependencies_ThenShouldNotDependOnOtherHandlers()
    {
        Classes()
            .That()
            .ResideInNamespaceMatching(IntegrationEventHandlerNamespace)
            .And()
            .HaveNameEndingWith("Handler")
            .Should()
            .FollowCustomCondition(
                cls =>
                {
                    var otherHandlerDeps = cls
                        .Dependencies.Select(d => d.Target)
                        .Where(t => t.Name.EndsWith("Handler") && t.FullName != cls.FullName);
                    return !otherHandlerDeps.Any();
                },
                "should not depend on other handlers",
                "Integration event handlers should be independent units. Cross-handler calls create hidden coupling and violate single responsibility."
            )
            .Check(Architecture);
    }

    [Test]
    public void GivenIntegrationEventHandlers_WhenCheckingDependencies_ThenShouldNotDependOnIMediator()
    {
        Classes()
            .That()
            .ResideInNamespaceMatching(IntegrationEventHandlerNamespace)
            .And()
            .HaveNameEndingWith("Handler")
            .Should()
            .FollowCustomCondition(
                cls => !cls.Dependencies.Select(d => d.Target).Any(t => t.Name == "IMediator"),
                "should not depend on IMediator",
                "Wolverine integration event handlers should cascade messages via return values or IMessageBus method injection. Injecting Mediator's IMediator mixes two message-passing mechanisms."
            )
            .Check(Architecture);
    }
}
