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
    [Test]
    public async Task GivenNoUnsentEmails_WhenHandling_ThenShouldNotSendOrSave()
    {
        var logBufferMock = new Mock<GlobalLogBuffer>();
        var logger = Mock.Of<ILogger<ResendErrorEmailIntegrationEventHandler>>();
        var repositoryMock = new Mock<IOutboxRepository>();
        var senderMock = new Mock<ISender>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        repositoryMock
            .Setup(x =>
                x.ListAsync(It.IsAny<ISpecification<Outbox>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);

        await new ResendErrorEmailIntegrationEventHandler(
            logger,
            logBufferMock.Object,
            repositoryMock.Object,
            senderMock.Object
        ).Handle(new ResendErrorEmailIntegrationEvent(), CancellationToken.None);

        senderMock.Verify(
            x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GivenUnsentEmails_WhenHandling_ThenShouldSendEachEmailAndMarkAsSent()
    {
        var logBufferMock = new Mock<GlobalLogBuffer>();
        var logger = Mock.Of<ILogger<ResendErrorEmailIntegrationEventHandler>>();
        var repositoryMock = new Mock<IOutboxRepository>();
        var senderMock = new Mock<ISender>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        repositoryMock.Setup(x => x.UnitOfWork).Returns(unitOfWorkMock.Object);

        var email1 = new Outbox("User1", "user1@test.com", "Sub1", "Body1");
        var email2 = new Outbox("User2", "user2@test.com", "Sub2", "Body2");

        repositoryMock
            .Setup(x =>
                x.ListAsync(It.IsAny<ISpecification<Outbox>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([email1, email2]);

        await new ResendErrorEmailIntegrationEventHandler(
            logger,
            logBufferMock.Object,
            repositoryMock.Object,
            senderMock.Object
        ).Handle(new ResendErrorEmailIntegrationEvent(), CancellationToken.None);

        senderMock.Verify(
            x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
        email1.IsSent.ShouldBeTrue();
        email2.IsSent.ShouldBeTrue();
    }

    [Test]
    public async Task GivenUnsentEmails_WhenHandling_ThenShouldSaveChangesAfterSuccessfulSend()
    {
        var logBufferMock = new Mock<GlobalLogBuffer>();
        var logger = Mock.Of<ILogger<ResendErrorEmailIntegrationEventHandler>>();
        var repositoryMock = new Mock<IOutboxRepository>();
        var senderMock = new Mock<ISender>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        repositoryMock.Setup(x => x.UnitOfWork).Returns(unitOfWorkMock.Object);

        var email = new Outbox("User1", "user1@test.com", "Sub1", "Body1");

        repositoryMock
            .Setup(x =>
                x.ListAsync(It.IsAny<ISpecification<Outbox>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([email]);

        await new ResendErrorEmailIntegrationEventHandler(
            logger,
            logBufferMock.Object,
            repositoryMock.Object,
            senderMock.Object
        ).Handle(new ResendErrorEmailIntegrationEvent(), CancellationToken.None);

        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenAllSendsFail_WhenHandling_ThenShouldNotSaveChangesAndFlushLogBuffer()
    {
        var logBufferMock = new Mock<GlobalLogBuffer>();
        var logger = Mock.Of<ILogger<ResendErrorEmailIntegrationEventHandler>>();
        var repositoryMock = new Mock<IOutboxRepository>();
        var senderMock = new Mock<ISender>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var email = new Outbox("User1", "user1@test.com", "Sub1", "Body1");

        repositoryMock
            .Setup(x =>
                x.ListAsync(It.IsAny<ISpecification<Outbox>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([email]);

        senderMock
            .Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SMTP connection failed"));

        await new ResendErrorEmailIntegrationEventHandler(
            logger,
            logBufferMock.Object,
            repositoryMock.Object,
            senderMock.Object
        ).Handle(new ResendErrorEmailIntegrationEvent(), CancellationToken.None);

        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        logBufferMock.Verify(x => x.Flush(), Times.Once);
    }

    [Test]
    public async Task GivenMixedSendResults_WhenHandling_ThenShouldSaveChangesAndFlushLogBuffer()
    {
        var logBufferMock = new Mock<GlobalLogBuffer>();
        var logger = Mock.Of<ILogger<ResendErrorEmailIntegrationEventHandler>>();
        var repositoryMock = new Mock<IOutboxRepository>();
        var senderMock = new Mock<ISender>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        repositoryMock.Setup(x => x.UnitOfWork).Returns(unitOfWorkMock.Object);

        var successEmail = new Outbox("User1", "user1@test.com", "Sub1", "Body1");
        var failureEmail = new Outbox("User2", "user2@test.com", "Sub2", "Body2");

        repositoryMock
            .Setup(x =>
                x.ListAsync(It.IsAny<ISpecification<Outbox>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([successEmail, failureEmail]);

        senderMock
            .SetupSequence(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .ThrowsAsync(new InvalidOperationException("SMTP connection failed"));

        await new ResendErrorEmailIntegrationEventHandler(
            logger,
            logBufferMock.Object,
            repositoryMock.Object,
            senderMock.Object
        ).Handle(new ResendErrorEmailIntegrationEvent(), CancellationToken.None);

        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        logBufferMock.Verify(x => x.Flush(), Times.Once);
        successEmail.IsSent.ShouldBeTrue();
        failureEmail.IsSent.ShouldBeFalse();
    }

    [Test]
    public async Task GivenCancellationRequested_WhenHandling_ThenShouldPropagateOperationCanceledException()
    {
        var logBufferMock = new Mock<GlobalLogBuffer>();
        var logger = Mock.Of<ILogger<ResendErrorEmailIntegrationEventHandler>>();
        var repositoryMock = new Mock<IOutboxRepository>();
        var senderMock = new Mock<ISender>();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var email = new Outbox("User1", "user1@test.com", "Sub1", "Body1");

        repositoryMock
            .Setup(x =>
                x.ListAsync(It.IsAny<ISpecification<Outbox>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([email]);

        senderMock
            .Setup(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException(cts.Token));

        await Should.ThrowAsync<OperationCanceledException>(() =>
            new ResendErrorEmailIntegrationEventHandler(
                logger,
                logBufferMock.Object,
                repositoryMock.Object,
                senderMock.Object
            ).Handle(new ResendErrorEmailIntegrationEvent(), cts.Token)
        );
    }
}
