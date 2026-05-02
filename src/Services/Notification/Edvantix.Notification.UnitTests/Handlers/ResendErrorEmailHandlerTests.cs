using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;
using Edvantix.Contracts;
using Edvantix.Notification.Domain.Models;
using Edvantix.Notification.Infrastructure.Senders;
using Edvantix.Notification.IntegrationEvents.EventHandlers;
using Microsoft.Extensions.Diagnostics.Buffering;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Edvantix.Notification.UnitTests.Handlers;

public sealed class ResendErrorEmailHandlerTests
{
    private readonly Mock<GlobalLogBuffer> _logBufferMock = new();
    private readonly ILogger _logger = Mock.Of<ILogger>();
    private readonly Mock<IOutboxRepository> _repositoryMock = new();
    private readonly Mock<ISender> _senderMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public ResendErrorEmailHandlerTests()
    {
        _repositoryMock.Setup(x => x.UnitOfWork).Returns(_unitOfWorkMock.Object);
    }

    [Test]
    public async Task GivenNoUnsentEmails_WhenHandling_ThenShouldNotSendOrSave()
    {
        _repositoryMock
            .Setup(x =>
                x.ListAsync(It.IsAny<ISpecification<Outbox>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);

        await ResendErrorEmailIntegrationEventHandler.Handle(
            new ResendErrorEmailIntegrationEvent(),
            _logger,
            _logBufferMock.Object,
            _repositoryMock.Object,
            _senderMock.Object,
            CancellationToken.None
        );

        _senderMock.Verify(
            x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GivenUnsentEmails_WhenHandling_ThenShouldSendEachEmailAndMarkAsSent()
    {
        var email1 = new Outbox("User1", "user1@test.com", "Sub1", "Body1");
        var email2 = new Outbox("User2", "user2@test.com", "Sub2", "Body2");

        _repositoryMock
            .Setup(x =>
                x.ListAsync(It.IsAny<ISpecification<Outbox>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([email1, email2]);

        await ResendErrorEmailIntegrationEventHandler.Handle(
            new ResendErrorEmailIntegrationEvent(),
            _logger,
            _logBufferMock.Object,
            _repositoryMock.Object,
            _senderMock.Object,
            CancellationToken.None
        );

        _senderMock.Verify(
            x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
        email1.IsSent.ShouldBeTrue();
        email2.IsSent.ShouldBeTrue();
    }

    [Test]
    public async Task GivenUnsentEmails_WhenHandling_ThenShouldSaveChangesAfterSuccessfulSend()
    {
        var email = new Outbox("User1", "user1@test.com", "Sub1", "Body1");

        _repositoryMock
            .Setup(x =>
                x.ListAsync(It.IsAny<ISpecification<Outbox>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([email]);

        await ResendErrorEmailIntegrationEventHandler.Handle(
            new ResendErrorEmailIntegrationEvent(),
            _logger,
            _logBufferMock.Object,
            _repositoryMock.Object,
            _senderMock.Object,
            CancellationToken.None
        );

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenAllSendsFail_WhenHandling_ThenShouldNotSaveChangesAndFlushLogBuffer()
    {
        var email = new Outbox("User1", "user1@test.com", "Sub1", "Body1");

        _repositoryMock
            .Setup(x =>
                x.ListAsync(It.IsAny<ISpecification<Outbox>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([email]);

        _senderMock
            .Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SMTP connection failed"));

        await ResendErrorEmailIntegrationEventHandler.Handle(
            new ResendErrorEmailIntegrationEvent(),
            _logger,
            _logBufferMock.Object,
            _repositoryMock.Object,
            _senderMock.Object,
            CancellationToken.None
        );

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _logBufferMock.Verify(x => x.Flush(), Times.Once);
    }

    [Test]
    public async Task GivenMixedSendResults_WhenHandling_ThenShouldSaveChangesAndFlushLogBuffer()
    {
        var successEmail = new Outbox("User1", "user1@test.com", "Sub1", "Body1");
        var failureEmail = new Outbox("User2", "user2@test.com", "Sub2", "Body2");

        _repositoryMock
            .Setup(x =>
                x.ListAsync(It.IsAny<ISpecification<Outbox>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([successEmail, failureEmail]);

        _senderMock
            .SetupSequence(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .ThrowsAsync(new InvalidOperationException("SMTP connection failed"));

        await ResendErrorEmailIntegrationEventHandler.Handle(
            new ResendErrorEmailIntegrationEvent(),
            _logger,
            _logBufferMock.Object,
            _repositoryMock.Object,
            _senderMock.Object,
            CancellationToken.None
        );

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _logBufferMock.Verify(x => x.Flush(), Times.Once);
        successEmail.IsSent.ShouldBeTrue();
        failureEmail.IsSent.ShouldBeFalse();
    }

    [Test]
    public async Task GivenCancellationRequested_WhenHandling_ThenShouldPropagateOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var email = new Outbox("User1", "user1@test.com", "Sub1", "Body1");

        _repositoryMock
            .Setup(x =>
                x.ListAsync(It.IsAny<ISpecification<Outbox>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([email]);

        _senderMock
            .Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException(cts.Token));

        await Should.ThrowAsync<OperationCanceledException>(() =>
            ResendErrorEmailIntegrationEventHandler.Handle(
                new ResendErrorEmailIntegrationEvent(),
                _logger,
                _logBufferMock.Object,
                _repositoryMock.Object,
                _senderMock.Object,
                cts.Token
            )
        );
    }
}
