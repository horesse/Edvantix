namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Teachers;

public sealed class GetTeachersEndpointTests
{
    private readonly GetTeachersEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenRequest_WhenHandling_ThenShouldSendQueryToSender()
    {
        var request = new GetTeachersQuery(Search: "Иванов");
        _senderMock
            .Setup(s =>
                s.Send(
                    It.Is<GetTeachersQuery>(q => q.Search == request.Search),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(Array.Empty<TeacherDto>());

        await _endpoint.HandleAsync(request, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(
                    It.Is<GetTeachersQuery>(q => q.Search == request.Search),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GivenRequest_WhenHandling_ThenShouldReturnOkWithCollection()
    {
        IReadOnlyCollection<TeacherDto> teachers =
        [
            new(Guid.CreateVersion7(), "Иванов Иван", "Преподаватель", null),
        ];
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetTeachersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teachers);

        var result = await _endpoint.HandleAsync(new GetTeachersQuery(), _senderMock.Object);

        result.ShouldBeOfType<Ok<IReadOnlyCollection<TeacherDto>>>();
        result.Value.ShouldBe(teachers);
    }

    [Test]
    public async Task GivenEmptyResult_WhenHandling_ThenShouldReturnOkWithEmptyCollection()
    {
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetTeachersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TeacherDto>());

        var result = await _endpoint.HandleAsync(new GetTeachersQuery(), _senderMock.Object);

        result.ShouldBeOfType<Ok<IReadOnlyCollection<TeacherDto>>>();
        result.Value.ShouldBeEmpty();
    }
}
