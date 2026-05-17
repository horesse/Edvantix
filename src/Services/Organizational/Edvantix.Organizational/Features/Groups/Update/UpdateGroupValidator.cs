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

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название группы обязательно")
            .MaximumLength(512)
            .WithMessage("Название группы не может превышать 512 символов");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Описание группы обязательно")
            .MaximumLength(1024)
            .WithMessage("Описание группы не может превышать 1024 символа");

        RuleFor(x => x.LevelId).NotEmpty().WithMessage("Идентификатор уровня обязателен");

        RuleFor(x => x.LevelId)
            .MustAsync(async (id, ct) =>
                await levels.ExistsAsync(id, tenantContext.OrganizationId, requireActive: true, ct)
            )
            .WithMessage("Уровень не найден или деактивирован.")
            .When(x => x.LevelId != Guid.Empty);

        // CourseId cross-context validation is intentionally left to the handler,
        // which skips the gRPC call when the value has not changed.
        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Идентификатор курса обязателен");

        RuleFor(x => x.TeacherMemberId)
            .NotEmpty()
            .WithMessage("Идентификатор преподавателя обязателен");

        RuleFor(x => x.TeacherMemberId)
            .MustAsync(async (id, ct) =>
                await members.ExistsAsync(id, tenantContext.OrganizationId, ct)
            )
            .WithMessage("Преподаватель не найден.")
            .When(x => x.TeacherMemberId != Guid.Empty);

        RuleFor(x => x.Capacity)
            .InclusiveBetween(1, 50)
            .WithMessage("Вместимость группы должна быть от 1 до 50 участников");

        RuleFor(x => x.RoomId)
            .NotEmpty()
            .WithMessage("Кабинет обязателен при очном или смешанном формате")
            .When(x => x.Format is GroupFormat.Offline or GroupFormat.Mixed);

        RuleFor(x => x.RoomId)
            .MustAsync(async (id, ct) =>
                await rooms.ExistsAsync(id!.Value, tenantContext.OrganizationId, ct)
            )
            .WithMessage("Кабинет не найден.")
            .When(x => x.Format != GroupFormat.Online && x.RoomId.HasValue);

        RuleFor(x => x.Platform)
            .NotNull()
            .WithMessage("Онлайн-платформа обязательна при онлайн или смешанном формате")
            .When(x => x.Format is GroupFormat.Online or GroupFormat.Mixed);
    }
}
