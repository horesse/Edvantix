using Microsoft.AspNetCore.Http;

namespace Edvantix.Organizational.UnitTests.Features.Groups.Members.BulkAdd;

public sealed class BulkAddGroupMembersEndpointTests
{
    private readonly BulkAddGroupMembersEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenAllAdded_WhenHandling_ThenShouldReturn200Ok()
    {
        var groupId = Guid.CreateVersion7();
        var command = new BulkAddGroupMembersCommand(
            groupId,
            [
                new BulkAddItem(
                    Guid.CreateVersion7(),
                    GroupMemberRole.Student,
                    new DateOnly(2025, 9, 1)
                ),
            ]
        );
        var bulkResult = new BulkAddResult([Guid.CreateVersion7()], []);

        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bulkResult);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        result.ShouldBeOfType<Ok<BulkAddResult>>();
    }

    [Test]
    public async Task GivenPartialSuccess_WhenHandling_ThenShouldReturn207MultiStatus()
    {
        var groupId = Guid.CreateVersion7();
        var command = new BulkAddGroupMembersCommand(
            groupId,
            [
                new BulkAddItem(
                    Guid.CreateVersion7(),
                    GroupMemberRole.Student,
                    new DateOnly(2025, 9, 1)
                ),
                new BulkAddItem(
                    Guid.CreateVersion7(),
                    GroupMemberRole.Student,
                    new DateOnly(2025, 9, 1)
                ),
            ]
        );
        var bulkResult = new BulkAddResult(
            [Guid.CreateVersion7()],
            [new BulkAddFailure(Guid.CreateVersion7(), "Не является участником организации.")]
        );

        _senderMock
            .Setup(s => s.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bulkResult);

        var result = await _endpoint.HandleAsync(command, _senderMock.Object);

        var jsonResult = result.ShouldBeOfType<JsonHttpResult<BulkAddResult>>();
        jsonResult.StatusCode.ShouldBe(StatusCodes.Status207MultiStatus);
    }
}
