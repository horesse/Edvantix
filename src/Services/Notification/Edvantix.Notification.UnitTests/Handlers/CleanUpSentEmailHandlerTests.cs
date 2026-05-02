using Edvantix.Chassis.Repository;
using Edvantix.Contracts;
using Edvantix.Notification.Domain.Models;
using Edvantix.Notification.IntegrationEvents.EventHandlers;
using Microsoft.Extensions.Diagnostics.Buffering;
using Microsoft.Extensions.Logging;

namespace Edvantix.Notification.UnitTests.Handlers;

public sealed class CleanUpSentEmailHandlerTests
{
    private readonly Mock<GlobalLogBuffer> _logBufferMock = new();
    private readonly ILogger _logger = Mock.Of<ILogger>();
    private readonly Mock<IOutboxRepository> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public CleanUpSentEmailHandlerTests()
    {
        _repositoryMock.Setup(x => x.UnitOfWork).Returns(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task GivenSentEmails_WhenHandling_ThenShouldDeleteAndSaveChanges()
    {
        var email1 = new Outbox("User1", "user1@test.com", "Sub1", "Body1");
        email1.MarkAsSent();
        var email2 = new Outbox("User2", "user2@test.com", "Sub2", "Body2");
        email2.MarkAsSent();

        _repositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OutboxFilterSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([email1, email2]);

        await CleanUpSentEmailIntegrationEventHandler.Handle(
            new CleanUpSentEmailIntegrationEvent(),
            _logger,
            _logBufferMock.Object,
            _repositoryMock.Object,
            CancellationToken.None
        );

        _repositoryMock.Verify(
            x => x.BulkDelete(It.Is<IEnumerable<Outbox>>(e => e.Count() == 2)),
            Times.Once
        );
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenNoSentEmails_WhenHandling_ThenShouldNotDeleteOrSave()
    {
        _repositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OutboxFilterSpec>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        await CleanUpSentEmailIntegrationEventHandler.Handle(
            new CleanUpSentEmailIntegrationEvent(),
            _logger,
            _logBufferMock.Object,
            _repositoryMock.Object,
            CancellationToken.None
        );

        _repositoryMock.Verify(x => x.BulkDelete(It.IsAny<IEnumerable<Outbox>>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GivenRepositoryThrows_WhenHandling_ThenShouldFlushAndThrowInvalidOperationException()
    {
        _repositoryMock
            .Setup(x => x.ListAsync(It.IsAny<OutboxFilterSpec>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB error"));

        var exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            CleanUpSentEmailIntegrationEventHandler.Handle(
                new CleanUpSentEmailIntegrationEvent(),
                _logger,
                _logBufferMock.Object,
                _repositoryMock.Object,
                CancellationToken.None
            )
        );

        exception.Message.ShouldBe("Failed to clean up sent emails");
        _logBufferMock.Verify(x => x.Flush(), Times.Once);
    }
}
