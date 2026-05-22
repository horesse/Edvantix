using System.Text.RegularExpressions;
using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Groups.Features.Directories.Levels;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Directories.Levels.Create;

/// <summary>Создать запись справочника «Уровни».</summary>
[Transactional]
[RequirePermission(LevelPermissions.Manage)]
public sealed record CreateLevelDirectoryCommand(string Name, short Order, string? Description)
    : ICommand<LevelDirectoryDto>;

internal sealed class CreateLevelDirectoryCommandHandler(
    ITenantContext tenantContext,
    ILevelRepository repository
) : ICommandHandler<CreateLevelDirectoryCommand, LevelDirectoryDto>
{
    private static readonly Regex InvalidChars = new(@"[^A-Z0-9]", RegexOptions.Compiled);

    public async ValueTask<LevelDirectoryDto> Handle(
        CreateLevelDirectoryCommand command,
        CancellationToken cancellationToken
    )
    {
        var orgId = tenantContext.OrganizationId;
        var code = await ResolveUniqueCodeAsync(command.Name, orgId, cancellationToken);

        var level = new Level(
            orgId,
            LevelCode.From(code),
            command.Name,
            command.Description,
            LevelTone.Slate,
            command.Order
        );

        await repository.AddAsync(level, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return LevelDirectoryMapper.ToDto(level);
    }

    private async Task<string> ResolveUniqueCodeAsync(string name, Guid orgId, CancellationToken ct)
    {
        // Generate a human-readable code from the name, then ensure uniqueness.
        var baseCode = InvalidChars.Replace(name.Trim().ToUpperInvariant(), "_").Trim('_');

        baseCode = string.IsNullOrEmpty(baseCode)
            ? "LVL"
            : baseCode[..Math.Min(baseCode.Length, 12)];

        var candidate = baseCode;
        for (var i = 1; i <= 99; i++)
        {
            if (!await repository.ExistsWithCodeAsync(orgId, candidate, ct))
                return candidate;

            var suffix = $"_{i}";
            candidate = $"{baseCode[..Math.Min(baseCode.Length, 16 - suffix.Length)]}{suffix}";
        }

        // Safety fallback: use a short random hex code.
        return Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
    }
}
