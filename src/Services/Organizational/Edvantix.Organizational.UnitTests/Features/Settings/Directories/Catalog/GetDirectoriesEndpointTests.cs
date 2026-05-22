using Edvantix.Organizational.Features.Settings.Directories;
using Edvantix.Organizational.Features.Settings.Directories.Catalog;

namespace Edvantix.Organizational.UnitTests.Features.Settings.Directories.Catalog;

public sealed class GetDirectoriesEndpointTests
{
    private readonly GetDirectoriesEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldCallSenderOnce()
    {
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetDirectoriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleCatalog());

        await _endpoint.HandleAsync(_senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<GetDirectoriesQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenShouldReturnOkWithCatalog()
    {
        var catalog = CreateSampleCatalog();
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetDirectoriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(catalog);

        var result = await _endpoint.HandleAsync(_senderMock.Object);

        result.Value.ShouldBe(catalog);
    }

    [Test]
    public async Task GivenValidRequest_WhenHandling_ThenResultShouldContain8Items()
    {
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetDirectoriesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSampleCatalog());

        var result = await _endpoint.HandleAsync(_senderMock.Object);

        result.Value!.Count.ShouldBe(8);
    }

    private static IReadOnlyList<DirectorySummaryDto> CreateSampleCatalog() =>
        DirectoryCatalog
            .All.Select(d => new DirectorySummaryDto(
                d.Code,
                d.Name,
                d.Description,
                d.Icon,
                d.Badge,
                ActiveCount: 0,
                ArchivedCount: 0,
                LastModifiedAt: null,
                IsAvailable: false
            ))
            .ToList();
}
