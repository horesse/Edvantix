using Edvantix.Organizations.Grpc.Services;

namespace Edvantix.Organizations.Features.GroupMembership.AddStudentToGroup;

/// <summary>
/// Handles adding a student to a group.
/// Validates the student's Persona profile exists before modifying the aggregate,
/// following the fail-fast pattern from AssignRoleCommandHandler.
/// The operation is idempotent: if the student is already a member, the call succeeds silently.
/// </summary>
public sealed class AddStudentToGroupCommandHandler(
    IGroupRepository groupRepository,
    IPersonaProfileService personaProfileService
) : ICommandHandler<AddStudentToGroupCommand, Unit>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> Handle(
        AddStudentToGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        // Validate that the student profile exists in Persona before touching the aggregate.
        var profileExists = await personaProfileService.ValidateProfileExistsAsync(
            request.ProfileId,
            cancellationToken
        );

        if (!profileExists)
        {
            throw NotFoundException.For<Domain.AggregatesModel.GroupAggregate.GroupMembership>(
                request.ProfileId
            );
        }

        var group =
            await groupRepository.FindAsync(
                new GroupByIdSpecification(request.GroupId, includeMembers: true),
                cancellationToken
            ) ?? throw NotFoundException.For<Group>(request.GroupId);

        // AddMember is idempotent — no-op if already a member (SCH-06).
        group.AddMember(request.ProfileId, DateTimeOffset.UtcNow);

        await groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return default;
    }
}
