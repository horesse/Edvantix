namespace Edvantix.Organizations.Features.Groups.UpdateGroup;

/// <summary>Command to update an existing group's properties.</summary>
public sealed class UpdateGroupCommand : ICommand<Unit>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int MaxCapacity { get; init; }
    public required string Color { get; init; }
}
