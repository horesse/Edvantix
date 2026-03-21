namespace Edvantix.Organizations.Features.Groups.CreateGroup;

/// <summary>Command to create a new group scoped to the current tenant (school).</summary>
public sealed class CreateGroupCommand : ICommand<Guid>
{
    public required string Name { get; init; }
    public required int MaxCapacity { get; init; }
    public required string Color { get; init; }
}
