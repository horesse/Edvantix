namespace Edvantix.Scheduling.Features.LessonSlots.DeleteLessonSlot;

/// <summary>
/// Command to hard-delete a lesson slot.
/// <para>
/// <see cref="LessonSlot"/> does not implement <c>ISoftDelete</c> — slots are hard-deleted.
/// The repository's <c>Remove</c> method issues a DELETE statement to the database.
/// </para>
/// </summary>
public sealed class DeleteLessonSlotCommand : ICommand<Unit>
{
    /// <summary>The identifier of the lesson slot to delete.</summary>
    public required Guid Id { get; init; }
}

/// <summary>
/// Handles <see cref="DeleteLessonSlotCommand"/>.
/// Finds the slot and hard-deletes it. Throws <see cref="NotFoundException"/> if the slot does not exist.
/// </summary>
public sealed class DeleteLessonSlotCommandHandler(
    ILessonSlotRepository slotRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<DeleteLessonSlotCommand, Unit>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> Handle(
        DeleteLessonSlotCommand command,
        CancellationToken cancellationToken
    )
    {
        var slot =
            await slotRepository.FindByIdAsync(command.Id, cancellationToken)
            ?? throw NotFoundException.For<LessonSlot>(command.Id);

        // Hard delete — LessonSlot does not implement ISoftDelete (per plan spec).
        // The tenant query filter on DbContext ensures only slots belonging to
        // the current school can be found and deleted.
        slotRepository.Remove(slot);
        await unitOfWork.SaveEntitiesAsync(cancellationToken);

        return default;
    }
}
