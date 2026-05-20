using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Groups.Grpc.Services.Courses;

namespace Edvantix.Groups.Features.Groups.Create;

internal sealed class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupValidator(
        ILevelRepository levels,
        ICurriculumService curriculum,
        ITenantContext tenantContext
    )
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Код группы обязателен")
            .MaximumLength(32)
            .WithMessage("Код группы не может превышать 32 символа")
            .Matches(@"^[A-Z0-9\-]+$")
            .WithMessage(
                "Код группы должен содержать только заглавные латинские буквы, цифры и дефисы"
            );

        RuleFor(x => x.Name).GroupNameRules();
        RuleFor(x => x.Description).GroupDescriptionRules();

        RuleFor(x => x.LevelId).NotEmpty().WithMessage("Идентификатор уровня обязателен");
        RuleFor(x => x.LevelId)
            .MustBeActiveLevelInCurrentOrganization(levels, tenantContext)
            .When(x => x.LevelId != Guid.Empty);

        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Идентификатор курса обязателен");
        RuleFor(x => x.CourseId)
            .MustAsync(
                async (id, ct) =>
                {
                    var course = await curriculum.GetCourseByIdAsync(id.ToString(), ct);
                    return course is not null
                        && course.OrganizationId == tenantContext.OrganizationId.ToString();
                }
            )
            .WithMessage("Курс не найден или принадлежит другой организации.")
            .When(x => x.CourseId != Guid.Empty);

        RuleFor(x => x.TeacherMemberId)
            .NotEmpty()
            .WithMessage("Идентификатор преподавателя обязателен");

        RuleFor(x => x.Capacity).GroupCapacityRules();

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("Дата окончания должна быть позже даты начала");

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
