using FluentValidation.Results;

namespace Edvantix.Scheduling.Features.LessonSlots.EditLessonSlot;

/// <summary>
/// Handles <see cref="EditLessonSlotCommand"/>.
/// <para>
/// Execution order:
/// 1. Find the existing slot — throws <see cref="NotFoundException"/> if not found.
/// 2. Check for a global teacher conflict, excluding the current slot (D-06, self-exclusion).
///    Pass <c>command.Id</c> as <c>excludedSlotId</c> so a slot can be rescheduled to its own time.
/// 3. Apply teacher or time changes on the aggregate and persist.
/// </para>
/// </summary>
public sealed class EditLessonSlotCommandHandler(
    ILessonSlotRepository slotRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<EditLessonSlotCommand, Unit>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> Handle(
        EditLessonSlotCommand command,
        CancellationToken cancellationToken
    )
    {
        // Step 1: Load the aggregate — tenant query filter is applied by DbContext automatically.
        var slot =
            await slotRepository.FirstOrDefaultAsync(
                new LessonSlotByIdSpecification(command.Id),
                cancellationToken
            ) ?? throw NotFoundException.For<LessonSlot>(command.Id);

        // Step 2: Global teacher conflict check with self-exclusion (D-06).
        // Passing command.Id excludes the slot being edited from the overlap query so
        // rescheduling a slot to its own time range does not produce a false conflict.
        var hasConflict = await slotRepository.HasConflictAsync(
            command.TeacherId,
            command.StartTime,
            command.EndTime,
            command.Id,
            cancellationToken
        );

        if (hasConflict)
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(command.TeacherId),
                    "Teacher is already booked at the requested time"
                ),
            ]);
        }

        // Step 3: Apply aggregate changes — only call domain methods when the value differs
        // to avoid generating unnecessary domain events in future phases.
        if (slot.TeacherId != command.TeacherId)
        {
            slot.ChangeTeacher(command.TeacherId);
        }

        if (slot.StartTime != command.StartTime || slot.EndTime != command.EndTime)
        {
            slot.Reschedule(command.StartTime, command.EndTime);
        }

        await unitOfWork.SaveEntitiesAsync(cancellationToken);

        return default;
    }
}
