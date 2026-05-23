using Edvantix.Organizational.Features.Directories.StudentTags;
using Edvantix.Organizational.Features.Directories.StudentTags.List;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.List;

public sealed class ListStudentTagsEndpointTests
{
    private readonly ListStudentTagsEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenActiveQuery_WhenHandling_ThenShouldCallSenderOnce()
    {
        var query = new ListStudentTagsQuery();
        var pagedResult = new PagedResult<StudentTagListItemDto>([], 1, 50, 0);
        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        await _endpoint.HandleAsync(query, _senderMock.Object);

        _senderMock.Verify(s => s.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenActiveQuery_WhenHandling_ThenShouldReturnOkWithPagedResult()
    {
        var query = new ListStudentTagsQuery();
        var items = new List<StudentTagListItemDto>
        {
            new(Guid.CreateVersion7(), "VIP", "#FF5733", false, 0),
        };
        var pagedResult = new PagedResult<StudentTagListItemDto>(items, 1, 50, 1);
        _senderMock
            .Setup(s => s.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var result = await _endpoint.HandleAsync(query, _senderMock.Object);

        result.ShouldBeOfType<Ok<PagedResult<StudentTagListItemDto>>>();
        result.Value.ShouldBe(pagedResult);
    }
}
