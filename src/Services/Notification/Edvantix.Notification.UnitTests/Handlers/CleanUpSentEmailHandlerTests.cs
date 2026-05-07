using Edvantix.Chassis.Repository;
using Edvantix.Contracts;
using Edvantix.Notification.Domain.Models;
using Edvantix.Notification.IntegrationEvents.EventHandlers;
using Microsoft.Extensions.Diagnostics.Buffering;
using Microsoft.Extensions.Logging;

namespace Edvantix.Notification.UnitTests.Handlers;

public sealed class CleanUpSentEmailHandlerTests
{
    [Test]
    public async Task GivenSentEmails_WhenHandling_ThenShouldDeleteAndSaveChanges()
    {
        var logBufferMock = new Mock<GlobalLogBuffer>();
        var logger = Mock.Of<ILogger<CleanUpSentEmailIntegrationEventHandler>>();
        var repositoryMock = new Mock<IOutboxRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        repositoryMock.Setup(x => x.UnitOfWork).Returns(unitOfWorkMock.Object);

        var email1 = new Outbox("User1", "user1@test.com", "Sub1", "Body1");
        email1.MarkAsSent();
        var email2 = new Outbox("User2", "user2@test.com", "Sub2", "Body2");
        email2.MarkAsSent();

        repositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OutboxFilterSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([email1, email2]);

        await new CleanUpSentEmailIntegrationEventHandler(
            logger,
            logBufferMock.Object,
            repositoryMock.Object
        ).Handle(new CleanUpSentEmailIntegrationEvent(), CancellationToken.None);

        repositoryMock.Verify(
            x => x.BulkDelete(It.Is<IEnumerable<Outbox>>(e => e.Count() == 2)),
            Times.Once
        );
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenNoSentEmails_WhenHandling_ThenShouldNotDeleteOrSave()
    {
        var logBufferMock = new Mock<GlobalLogBuffer>();
        var logger = Mock.Of<ILogger<CleanUpSentEmailIntegrationEventHandler>>();
        var repositoryMock = new Mock<IOutboxRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        repositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OutboxFilterSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        await new CleanUpSentEmailIntegrationEventHandler(
            logger,
            logBufferMock.Object,
            repositoryMock.Object
        ).Handle(new CleanUpSentEmailIntegrationEvent(), CancellationToken.None);

        repositoryMock.Verify(x => x.BulkDelete(It.IsAny<IEnumerable<Outbox>>()), Times.Never);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GivenRepositoryThrows_WhenHandling_ThenShouldFlushAndThrowInvalidOperationException()
    {
        var logBufferMock = new Mock<GlobalLogBuffer>();
        var logger = Mock.Of<ILogger<CleanUpSentEmailIntegrationEventHandler>>();
        var repositoryMock = new Mock<IOutboxRepository>();

        repositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OutboxFilterSpec>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            new CleanUpSentEmailIntegrationEventHandler(
                logger,
                logBufferMock.Object,
                repositoryMock.Object
            ).Handle(new CleanUpSentEmailIntegrationEvent(), CancellationToken.None)
        );

        exception.Message.ShouldBe("Failed to clean up sent emails");
        logBufferMock.Verify(x => x.Flush(), Times.Once);
    }
}
