namespace Edvantix.Organizations.Features.Groups.DeleteGroup;

/// <summary>Command to soft-delete a group.</summary>
public sealed class DeleteGroupCommand : ICommand<Unit>
{
    public required Guid Id { get; init; }
}
