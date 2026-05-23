using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Features.Directories.StudentTags;
using Edvantix.Organizational.Features.Directories.StudentTags.GetById;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.GetById;

public sealed class GetStudentTagByIdEndpointTests
{
    private readonly GetStudentTagByIdEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenExistingTag_WhenHandling_ThenShouldReturnOkWithDto()
    {
        var id = Guid.CreateVersion7();
        var dto = CreateDto(id);
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<GetStudentTagByIdQuery>(q => q.Id == id),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        var ok = result.Result.ShouldBeOfType<Ok<StudentTagDto>>();
        ok.Value.ShouldBe(dto);
    }

    [Test]
    public async Task GivenTagNotFound_WhenHandling_ThenShouldReturnNotFound()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetStudentTagByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(NotFoundException.For<StudentTag>(id));

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.Result.ShouldBeOfType<NotFound>();
    }

    private static StudentTagDto CreateDto(Guid id) =>
        new(
            id,
            "VIP",
            "#FF5733",
            false,
            0,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null
        );
}
