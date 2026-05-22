using Edvantix.Groups.Features.Directories.LessonTypes.Create;
using Edvantix.Groups.Features.Directories.LessonTypes.GetById;

namespace Edvantix.Groups.UnitTests.Features.Directories.LessonTypes.Create;

public sealed class CreateLessonTypeEndpointTests
{
    private readonly CreateLessonTypeEndpoint _endpoint = new();
    private readonly LinkGenerator _linkGenerator = new Mock<LinkGenerator>().Object;
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenCallsSenderOnce()
    {
        var command = BuildValidCommand();
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildDto(command.OrganizationId));

        await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        _senderMock.Verify(s => s.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenReturnsCreatedWithDto()
    {
        var command = BuildValidCommand();
        var dto = BuildDto(command.OrganizationId);
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        result.ShouldBeOfType<Created<LessonTypeDto>>();
        result.Value.ShouldBe(dto);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenLocationContainsCreatedId()
    {
        var command = BuildValidCommand();
        var dto = BuildDto(command.OrganizationId);
        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object, _linkGenerator);

        result.Location!.ShouldContain(dto.Id.ToString());
    }

    private static CreateLessonTypeCommand BuildValidCommand() =>
        new(Guid.CreateVersion7(), "Урок", "LESSON", 45, "#3B82F6", null);

    private static LessonTypeDto BuildDto(Guid orgId) =>
        new(
            Guid.CreateVersion7(),
            "Урок",
            "LESSON",
            45,
            "#3B82F6",
            null,
            0,
            false,
            DateTime.UtcNow,
            null
        );
}
