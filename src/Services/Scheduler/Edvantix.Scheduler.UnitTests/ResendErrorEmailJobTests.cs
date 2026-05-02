using Edvantix.Contracts;
using Edvantix.Scheduler.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;
using Wolverine;

namespace Edvantix.Scheduler.UnitTests;

public sealed class ResendErrorEmailJobTests
{
    private readonly Mock<IMessageBus> _busMock = new();
    private readonly Mock<ILogger<ResendErrorEmailJob>> _loggerMock = new();
    private readonly ResendErrorEmailJob _job;

    public ResendErrorEmailJobTests()
    {
        _job = new(_busMock.Object, _loggerMock.Object);
    }

    [Test]
    public async Task GivenValidDependencies_WhenExecutingJob_ThenShouldPublishExactlyOneEvent()
    {
        var context = Mock.Of<IJobExecutionContext>(c =>
            c.CancellationToken == CancellationToken.None
        );

        await _job.Execute(context);

        _busMock.Verify(
            b =>
                b.PublishAsync(
                    It.IsAny<ResendErrorEmailIntegrationEvent>(),
                    It.IsAny<DeliveryOptions?>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenMultipleExecutions_WhenExecutingJobConcurrently_ThenShouldHandleEachExecutionIndependently()
    {
        var context = Mock.Of<IJobExecutionContext>(c =>
            c.CancellationToken == CancellationToken.None
        );
        const int numberOfExecutions = 3;

        var tasks = Enumerable
            .Range(0, numberOfExecutions)
            .Select(_ => _job.Execute(context))
            .ToArray();

        await Task.WhenAll(tasks);

        _busMock.Verify(
            b =>
                b.PublishAsync(
                    It.IsAny<ResendErrorEmailIntegrationEvent>(),
                    It.IsAny<DeliveryOptions?>()
                ),
            Times.Exactly(numberOfExecutions)
        );
    }

    [Test]
    public async Task GivenPublishThrows_WhenExecutingJob_ThenShouldWrapInJobExecutionException()
    {
        var context = Mock.Of<IJobExecutionContext>(c =>
            c.CancellationToken == CancellationToken.None
        );

        _busMock
            .Setup(x =>
                x.PublishAsync(
                    It.IsAny<ResendErrorEmailIntegrationEvent>(),
                    It.IsAny<DeliveryOptions?>()
                )
            )
            .ThrowsAsync(new InvalidOperationException("Bus failure"));

        var exception = await Should.ThrowAsync<JobExecutionException>(() => _job.Execute(context));

        exception.InnerException.ShouldBeOfType<InvalidOperationException>();
        exception.InnerException!.Message.ShouldBe("Bus failure");
    }
}
