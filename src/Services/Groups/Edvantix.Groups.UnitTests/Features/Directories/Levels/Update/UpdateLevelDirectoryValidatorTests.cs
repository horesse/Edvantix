namespace Edvantix.Groups.UnitTests.Features.Directories.Levels.Update;

public sealed class UpdateLevelDirectoryValidatorTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly UpdateLevelDirectoryValidator _validator;

    public UpdateLevelDirectoryValidatorTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        SetupUniqueName(isUnique: true);
        _validator = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldPass()
    {
        var result = await _validator.ValidateAsync(BuildCommand());

        result.IsValid.ShouldBeTrue();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyName_WhenValidating_ThenShouldFail(string? name)
    {
        var result = await _validator.ValidateAsync(BuildCommand(name: name!));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.PropertyName == nameof(UpdateLevelDirectoryCommand.Name)
        );
    }

    [Test]
    public async Task GivenNameExceeds64Chars_WhenValidating_ThenShouldFail()
    {
        var result = await _validator.ValidateAsync(BuildCommand(name: new string('A', 65)));

        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GivenDuplicateName_WhenValidating_ThenShouldFail()
    {
        SetupUniqueName(isUnique: false);
        var id = Guid.CreateVersion7();

        var result = await _validator.ValidateAsync(BuildCommand(id: id));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.PropertyName == nameof(UpdateLevelDirectoryCommand.Name)
        );
    }

    [Test]
    public async Task GivenNegativeOrder_WhenValidating_ThenShouldFail()
    {
        var result = await _validator.ValidateAsync(BuildCommand(order: -1));

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.PropertyName == nameof(UpdateLevelDirectoryCommand.Order)
        );
    }

    [Test]
    public async Task GivenDescriptionExceeds256Chars_WhenValidating_ThenShouldFail()
    {
        var result = await _validator.ValidateAsync(
            BuildCommand(description: new string('X', 257))
        );

        result.IsValid.ShouldBeFalse();
    }

    private void SetupUniqueName(bool isUnique) =>
        _repoMock
            .Setup(r =>
                r.ExistsWithNameAsync(
                    _organizationId,
                    It.IsAny<string>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(!isUnique);

    private static UpdateLevelDirectoryCommand BuildCommand(
        Guid? id = null,
        string name = "Beginner",
        short order = 1,
        string? description = null
    ) => new(id ?? Guid.CreateVersion7(), name, order, description);
}
