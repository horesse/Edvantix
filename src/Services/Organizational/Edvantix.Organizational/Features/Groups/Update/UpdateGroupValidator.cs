using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.Features.Groups.Update;

internal sealed class UpdateGroupValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupValidator(
        ILevelRepository levels,
        IOrganizationMemberRepository members,
        IRoomRepository rooms,
        ITenantContext tenantContext
    )
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Идентификатор группы обязателен");

        RuleFor(x => x.Name).GroupNameRules();
        RuleFor(x => x.Description).GroupDescriptionRules();

        RuleFor(x => x.LevelId).NotEmpty().WithMessage("Идентификатор уровня обязателен");
        RuleFor(x => x.LevelId)
            .MustBeActiveLevelInCurrentOrganization(levels, tenantContext)
            .When(x => x.LevelId != Guid.Empty);

        // CourseId cross-context validation is intentionally left to the handler,
        // which skips the gRPC call when the value has not changed.
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Идентификатор курса обязателен");

        RuleFor(x => x.TeacherMemberId)
            .NotEmpty()
            .WithMessage("Идентификатор преподавателя обязателен");
        RuleFor(x => x.TeacherMemberId)
            .MustBeMemberOfCurrentOrganization(members, tenantContext)
            .When(x => x.TeacherMemberId != Guid.Empty);

        RuleFor(x => x.Capacity).GroupCapacityRules();

        RuleFor(x => x.RoomId)
            .NotEmpty()
            .WithMessage("Кабинет обязателен при очном или смешанном формате")
            .When(x => x.Format is GroupFormat.Offline or GroupFormat.Mixed);
        RuleFor(x => x.RoomId)
            .MustExistAsRoomInCurrentOrganization(rooms, tenantContext)
            .When(x => x.Format != GroupFormat.Online && x.RoomId.HasValue);

        RuleFor(x => x.Platform)
            .NotNull()
            .WithMessage("Онлайн-платформа обязательна при онлайн или смешанном формате")
            .When(x => x.Format is GroupFormat.Online or GroupFormat.Mixed);
    }
}
