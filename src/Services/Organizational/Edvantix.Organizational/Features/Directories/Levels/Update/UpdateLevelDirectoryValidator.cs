using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Organizational.Features.Directories.Levels.Update;

internal sealed class UpdateLevelDirectoryValidator : AbstractValidator<UpdateLevelDirectoryCommand>
{
    public UpdateLevelDirectoryValidator(ITenantContext tenantContext, ILevelRepository repository)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название уровня обязательно.")
            .MaximumLength(64)
            .WithMessage("Название уровня не может превышать 64 символа.")
            .MustAsync(
                async (cmd, name, ct) =>
                    !await repository.ExistsWithNameAsync(
                        tenantContext.OrganizationId,
                        name,
                        excludeId: cmd.Id,
                        ct
                    )
            )
            .WithMessage("Уровень с таким названием уже существует в организации.");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo((short)0)
            .WithMessage("Порядковый номер не может быть отрицательным.");

        RuleFor(x => x.Description)
            .MaximumLength(256)
            .WithMessage("Описание уровня не может превышать 256 символов.")
            .When(x => x.Description is not null);
    }
}
