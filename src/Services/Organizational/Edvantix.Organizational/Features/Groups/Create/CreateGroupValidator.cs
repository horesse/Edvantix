using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Organizational.Features.Groups.Create;

internal sealed class CreateGroupValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupValidator(ILevelRepository levelRepository, ITenantContext tenantContext)
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

        RuleFor(x => x.LevelId)
            .NotEmpty()
            .WithMessage("Идентификатор уровня обязателен");

        When(x => x.LevelId != Guid.Empty, () =>
        {
            RuleFor(x => x.LevelId)
                .MustAsync(async (levelId, ct) =>
                {
                    var level = await levelRepository.GetByIdAsync(levelId, ct);
                    return level is not null && !level.IsDeleted;
                })
                .WithMessage("Указанный уровень не найден");

            RuleFor(x => x.LevelId)
                .MustAsync(async (levelId, ct) =>
                {
                    var level = await levelRepository.GetByIdAsync(levelId, ct);
                    return level?.OrganizationId == tenantContext.OrganizationId;
                })
                .WithMessage("Уровень не принадлежит текущей организации");

            RuleFor(x => x.LevelId)
                .MustAsync(async (levelId, ct) =>
                {
                    var level = await levelRepository.GetByIdAsync(levelId, ct);
                    return level?.IsActive == true;
                })
                .WithMessage("Уровень неактивен и не может быть назначен группе");
        });

        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Идентификатор курса обязателен");

        RuleFor(x => x.TeacherMemberId)
            .NotEmpty()
            .WithMessage("Идентификатор преподавателя обязателен");

        RuleFor(x => x.Capacity)
            .InclusiveBetween(1, 50)
            .WithMessage("Вместимость группы должна быть от 1 до 50 участников");

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
