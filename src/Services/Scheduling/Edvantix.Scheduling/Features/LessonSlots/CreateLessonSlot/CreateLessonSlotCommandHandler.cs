using Edvantix.Scheduling.Grpc.Services;
using FluentValidation.Results;

namespace Edvantix.Scheduling.Features.LessonSlots.CreateLessonSlot;

/// <summary>
/// Handles <see cref="CreateLessonSlotCommand"/>.
/// <para>
/// Execution order:
/// 1. Validate the group exists in the Organizations service via <see cref="IOrganizationsGroupService"/>.
/// 2. Check for a global teacher conflict across all tenants via <see cref="ILessonSlotRepository.HasConflictAsync"/>
///    with <c>excludedSlotId = null</c> (new slot has no existing ID to exclude — D-04, D-05).
/// 3. Create and persist the <see cref="LessonSlot"/> aggregate.
/// </para>
/// </summary>
public sealed class CreateLessonSlotCommandHandler(
    ILessonSlotRepository slotRepository,
    IOrganizationsGroupService groupService,
    ITenantContext tenantContext,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateLessonSlotCommand, Guid>
{
    /// <inheritdoc/>
    public async ValueTask<Guid> Handle(
        CreateLessonSlotCommand command,
        CancellationToken cancellationToken
    )
    {
        // Step 1: Validate cross-service group reference (D-15).
        // Groups are owned by the Organizations service — Scheduling stores only the GroupId Guid.
        var exists = await groupService.GroupExistsAsync(command.GroupId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException($"Group {command.GroupId} not found");
        }

        // Step 2: Global teacher conflict check (D-04, D-05).
        // Pass null as excludedSlotId — this is a new slot so there is nothing to self-exclude.
        var hasConflict = await slotRepository.HasConflictAsync(
            command.TeacherId,
            command.StartTime,
            command.EndTime,
            null,
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

        // Step 3: Create and persist the lesson slot.
        var slot = new LessonSlot(
            tenantContext.SchoolId,
            command.GroupId,
            command.TeacherId,
            command.StartTime,
            command.EndTime
        );

        slotRepository.Add(slot);
        await unitOfWork.SaveEntitiesAsync(cancellationToken);

        return slot.Id;
    }
}
