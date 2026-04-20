namespace Edvantix.Organizational.UnitTests.Features.Roles.Get;

public sealed class GetRoleEndpointTests
{
    private readonly GetRoleEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    private static RoleDetailDto BuildDto(Guid id) =>
        new(id, Guid.CreateVersion7(), "manager", "Менеджер", []);

    [Test]
    public async Task GivenExistingRole_WhenHandling_ThenShouldReturnOk()
    {
        var id = Guid.CreateVersion7();
        var dto = BuildDto(id);
        _senderMock
            .Setup(s => s.Send(new GetRoleQuery(id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.ShouldBeOfType<Ok<RoleDetailDto>>();
        result.Value.ShouldBe(dto);
    }

    [Test]
    public async Task GivenExistingRole_WhenHandling_ThenShouldCallSenderOnce()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(new GetRoleQuery(id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildDto(id));

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s => s.Send(new GetRoleQuery(id), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
