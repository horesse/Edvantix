namespace Edvantix.Organizational.UnitTests.Features.Levels.Reorder;

public sealed class ReorderLevelsValidatorTests
{
    private readonly ReorderLevelsValidator _validator = new();

    [Test]
    public void GivenValidItems_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var result = _validator.TestValidate(BuildValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void GivenEmptyItems_WhenValidating_ThenShouldHaveError()
    {
        var result = _validator.TestValidate(new ReorderLevelsCommand([]));

        result.ShouldHaveValidationErrorFor(x => x.Items);
    }

    [Test]
    public void GivenItemWithEmptyId_WhenValidating_ThenShouldHaveError()
    {
        var command = new ReorderLevelsCommand([new LevelOrderItem(Guid.Empty, SortOrder: 1)]);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Items[0].Id");
    }

    [Test]
    public void GivenItemWithNegativeSortOrder_WhenValidating_ThenShouldHaveError()
    {
        var command = new ReorderLevelsCommand([
            new LevelOrderItem(Guid.CreateVersion7(), SortOrder: -1),
        ]);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("Items[0].SortOrder");
    }

    [Test]
    public void GivenItemWithZeroSortOrder_WhenValidating_ThenShouldNotHaveError()
    {
        var command = new ReorderLevelsCommand([
            new LevelOrderItem(Guid.CreateVersion7(), SortOrder: 0),
        ]);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor("Items[0].SortOrder");
    }

    [Test]
    public void GivenMultipleValidItems_WhenValidating_ThenShouldNotHaveAnyErrors()
    {
        var command = new ReorderLevelsCommand([
            new LevelOrderItem(Guid.CreateVersion7(), SortOrder: 1),
            new LevelOrderItem(Guid.CreateVersion7(), SortOrder: 2),
            new LevelOrderItem(Guid.CreateVersion7(), SortOrder: 3),
        ]);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static ReorderLevelsCommand BuildValidCommand() =>
        new([
            new LevelOrderItem(Guid.CreateVersion7(), SortOrder: 1),
            new LevelOrderItem(Guid.CreateVersion7(), SortOrder: 2),
        ]);
}
