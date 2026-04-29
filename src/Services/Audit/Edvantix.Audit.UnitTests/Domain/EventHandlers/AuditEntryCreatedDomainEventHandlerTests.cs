namespace Edvantix.Audit.UnitTests.Domain.EventHandlers;

public sealed class AuditEntryCreatedDomainEventHandlerTests
{
    private readonly AuditEntryCreatedDomainEventHandler _handler = new();

    [Test]
    public async Task GivenAuditEntryCreatedEvent_WhenHandling_ThenShouldCompleteWithoutError()
    {
        var @event = new AuditEntryCreatedDomainEvent(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            AuditAction.Created,
            AuditEntityType.Organization
        );

        var act = async () => await _handler.Handle(@event, CancellationToken.None);

        await act.ShouldNotThrowAsync();
    }

    [Test]
    public async Task GivenCancelledToken_WhenHandling_ThenShouldCompleteWithoutError()
    {
        var @event = new AuditEntryCreatedDomainEvent(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            AuditAction.Deleted,
            AuditEntityType.OrganizationMember
        );
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var act = async () => await _handler.Handle(@event, cts.Token);

        await act.ShouldNotThrowAsync();
    }
}
