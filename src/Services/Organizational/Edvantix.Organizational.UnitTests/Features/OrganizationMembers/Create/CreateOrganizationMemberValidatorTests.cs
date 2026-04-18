namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Create;

public sealed class CreateOrganizationMemberValidatorTests
{
    private readonly CreateOrganizationMemberValidator _validator = new();

    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    [Test]
    public void GivenValidCommand_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(BuildValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyProfileId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { ProfileId = Guid.Empty });

        result.ShouldHaveValidationErrorFor(x => x.ProfileId);
    }

    [Test]
    public void GivenEmptyOrganizationMemberRoleId_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                OrganizationMemberRoleId = Guid.Empty,
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.OrganizationMemberRoleId);
    }

    [Test]
    public void GivenFutureStartDate_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(
            BuildValidCommand() with
            {
                StartDate = Today.AddDays(1),
            }
        );

        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Test]
    public void GivenTodayStartDate_WhenValidating_ThenShouldNotHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { StartDate = Today });

        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
    }

    [Test]
    public void GivenEndDateBeforeStartDate_WhenValidating_ThenShouldHaveError()
    {
        var command = BuildValidCommand() with { StartDate = Today, EndDate = Today.AddDays(-1) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Test]
    public void GivenEndDateEqualToStartDate_WhenValidating_ThenShouldNotHaveError()
    {
        var command = BuildValidCommand() with
        {
            StartDate = new DateOnly(2025, 1, 1),
            EndDate = new DateOnly(2025, 1, 1),
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
    }

    [Test]
    public void GivenNullEndDate_WhenValidating_ThenShouldNotHaveError()
    {
        var result = _validator.TestValidate(BuildValidCommand() with { EndDate = null });

        result.ShouldNotHaveValidationErrorFor(x => x.EndDate);
    }

    private static CreateOrganizationMemberCommand BuildValidCommand() =>
        new(
            ProfileId: Guid.CreateVersion7(),
            OrganizationMemberRoleId: Guid.CreateVersion7(),
            StartDate: new DateOnly(2025, 1, 1)
        );
}
