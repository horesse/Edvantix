using Edvantix.Organizational.Features.Settings.Directories;
using Edvantix.Organizational.Features.Settings.Directories.GetDirectories;

namespace Edvantix.Organizational.UnitTests.Features.Settings.Directories.GetDirectories;

public sealed class GetDirectoriesEndpointTests
{
    private readonly GetDirectoriesEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldCallSenderOnce()
    {
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetDirectoriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateStubSummaries());

        await _endpoint.HandleAsync(_senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<GetDirectoriesQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldReturnOkResult()
    {
        var summaries = CreateStubSummaries();
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetDirectoriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaries);

        var result = await _endpoint.HandleAsync(_senderMock.Object);

        result.ShouldBeOfType<Ok<IReadOnlyList<DirectorySummaryDto>>>();
    }

    [Test]
    public async Task GivenAllStubSummaries_WhenHandling_ThenShouldReturnAllItems()
    {
        var summaries = CreateStubSummaries();
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetDirectoriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaries);

        var result = await _endpoint.HandleAsync(_senderMock.Object);

        result.Value!.Count.ShouldBe(8);
    }

    [Test]
    public async Task GivenSummariesWithUnavailableItems_WhenHandling_ThenValueShouldContainUnavailableItems()
    {
        var summaries = CreateStubSummaries();
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetDirectoriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(summaries);

        var result = await _endpoint.HandleAsync(_senderMock.Object);

        result.Value!.ShouldAllBe(r => !r.IsAvailable);
    }

    private static IReadOnlyList<DirectorySummaryDto> CreateStubSummaries() =>
        DirectoryCatalog.All
            .Select(d => DirectorySummaryDto.From(
                d,
                new DirectoryStats(0, 0, null, IsAvailable: false)
            ))
            .ToList();
}
