using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.UnitTests.Features.Settings.Directories;

public sealed class OrganizationScopedLookupValidatorTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    private sealed record FakeCommand(Guid OrganizationId, string? Name, Guid? Id = null);

    private sealed class FakeValidator : OrganizationScopedLookupValidator<FakeCommand>
    {
        public FakeValidator(IUniqueNameChecker checker)
            : base(checker, c => c.OrganizationId, c => c.Name, c => c.Id) { }
    }

    private static (FakeValidator validator, Mock<IUniqueNameChecker> mock) CreateValidator(
        bool exists = false
    )
    {
        var mock = new Mock<IUniqueNameChecker>();
        mock.SetupGet(x => x.DirectoryCode).Returns("test-directory");
        mock.Setup(x =>
                x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(exists);
        return (new FakeValidator(mock.Object), mock);
    }

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator(exists: false);
        var command = new FakeCommand(OrgId, "Уровень A1");

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task GivenEmptyOrganizationId_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new FakeCommand(Guid.Empty, "Имя");

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(OrganizationScopedLookupValidator<FakeCommand>.OrganizationIdProperty);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyName_WhenValidating_ThenShouldFail(string? name)
    {
        var (validator, _) = CreateValidator();
        var command = new FakeCommand(OrgId, name);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(OrganizationScopedLookupValidator<FakeCommand>.NameProperty);
    }

    [Test]
    public async Task GivenNameLongerThanMax_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var name = new string('A', 121);
        var command = new FakeCommand(OrgId, name);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(OrganizationScopedLookupValidator<FakeCommand>.NameProperty);
    }

    [Test]
    public async Task GivenDuplicateName_WhenValidating_ThenShouldFail()
    {
        var (validator, mock) = CreateValidator(exists: true);
        var command = new FakeCommand(OrgId, "Дубликат");

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(OrganizationScopedLookupValidator<FakeCommand>.NameProperty);
        mock.Verify(
            x => x.ExistsAsync(OrgId, "Дубликат", null, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenUpdateCommandWithExcludedId_WhenValidating_ThenShouldPassExcludeToChecker()
    {
        var (validator, mock) = CreateValidator(exists: false);
        var id = Guid.CreateVersion7();
        var command = new FakeCommand(OrgId, "Имя", Id: id);

        await validator.TestValidateAsync(command);

        mock.Verify(
            x => x.ExistsAsync(OrgId, "Имя", id, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenNameAtMaxLength_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var name = new string('A', 120);
        var command = new FakeCommand(OrgId, name);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(OrganizationScopedLookupValidator<FakeCommand>.NameProperty);
    }

    [Test]
    public async Task GivenInvalidName_WhenValidating_ThenUniquenessCheckShouldBeSkipped()
    {
        var (validator, mock) = CreateValidator();
        var command = new FakeCommand(OrgId, "");

        await validator.TestValidateAsync(command);

        mock.Verify(
            x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}
