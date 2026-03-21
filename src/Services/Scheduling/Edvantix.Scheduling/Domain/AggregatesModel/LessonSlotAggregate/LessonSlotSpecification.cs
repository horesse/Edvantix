namespace Edvantix.Scheduling.Domain.AggregatesModel.LessonSlotAggregate;

/// <summary>
/// Finds a single lesson slot by its identifier.
/// Used by command handlers that need to load and mutate a specific slot.
/// </summary>
public sealed class LessonSlotByIdSpecification : Specification<LessonSlot>
{
    /// <summary>Initializes the specification for the given slot identifier.</summary>
    /// <param name="slotId">The lesson slot identifier.</param>
    public LessonSlotByIdSpecification(Guid slotId)
    {
        Query.Where(s => s.Id == slotId);
    }
}
