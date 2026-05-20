using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Groups.Features.Groups.Update;

internal sealed class UpdateGroupValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupValidator(ILevelRepository levels, ITenantContext tenantContext)
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

        RuleFor(x => x.Capacity).GroupCapacityRules();

        RuleFor(x => x.RoomId)
            .NotEmpty()
            .WithMessage("Кабинет обязателен при очном или смешанном формате")
            .When(x => x.Format is GroupFormat.Offline or GroupFormat.Mixed);

        RuleFor(x => x.Platform)
            .NotNull()
            .WithMessage("Онлайн-платформа обязательна при онлайн или смешанном формате")
            .When(x => x.Format is GroupFormat.Online or GroupFormat.Mixed);
    }
}
