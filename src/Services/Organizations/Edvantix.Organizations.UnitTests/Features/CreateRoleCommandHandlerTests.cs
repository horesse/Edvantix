using Edvantix.Organizations.Features.Roles.CreateRole;

namespace Edvantix.Organizations.UnitTests.Features;

/// <summary>Unit tests for <see cref="CreateRoleCommandHandler"/>.</summary>
public sealed class CreateRoleCommandHandlerTests
{
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly CreateRoleCommandHandler _handler;
    private readonly Guid _schoolId = Guid.CreateVersion7();

    public CreateRoleCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _roleRepositoryMock = new Mock<IRoleRepository>();
        _roleRepositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _roleRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role r, CancellationToken _) =>
            {
                // Simulate EF Core key generation that occurs before SaveChanges in the real repository
                r.Id = Guid.CreateVersion7();
                return r;
            });

        _tenantContextMock = new Mock<ITenantContext>();
        _tenantContextMock.SetupGet(t => t.SchoolId).Returns(_schoolId);

        _handler = new CreateRoleCommandHandler(_roleRepositoryMock.Object, _tenantContextMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenRoleCreatedWithTenantSchoolId()
    {
        var command = new CreateRoleCommand { Name = "Admin" };

        await _handler.Handle(command, CancellationToken.None);

        _roleRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Role>(role => role.SchoolId == _schoolId), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenReturnsNewRoleId()
    {
        var command = new CreateRoleCommand { Name = "Teacher" };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBe(Guid.Empty);
    }
}
