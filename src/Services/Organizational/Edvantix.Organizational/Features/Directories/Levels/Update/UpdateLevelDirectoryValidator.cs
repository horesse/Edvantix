using System.Text.RegularExpressions;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Organizational.Features.Directories.Levels.Update;

internal sealed class UpdateLevelDirectoryValidator : AbstractValidator<UpdateLevelDirectoryCommand>
{
    private static readonly Regex CodePattern = new(
        @"^[A-Z0-9_-]{1,16}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100)
    );

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

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Код уровня обязателен.")
            .Must(c => CodePattern.IsMatch(c.Trim().ToUpperInvariant()))
            .WithMessage(
                "Код может содержать только латинские буквы, цифры, дефисы и подчёркивания (до 16 символов)."
            )
            .MustAsync(
                async (cmd, code, ct) =>
                    !await repository.ExistsWithCodeAsync(
                        tenantContext.OrganizationId,
                        code,
                        excludeId: cmd.Id,
                        ct
                    )
            )
            .WithMessage("Уровень с таким кодом уже существует в организации.")
            .When(x => !string.IsNullOrWhiteSpace(x.Code));
    }
}
