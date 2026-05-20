using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Groups.Domain.Permissions;

namespace Edvantix.Groups.Features.Groups.SuggestCode;

[RequirePermission(GroupPermissions.View)]
public sealed record GetSuggestedGroupCodeQuery(Guid LevelId) : IQuery<string>;

internal sealed class GetSuggestedGroupCodeQueryHandler(
    ITenantContext tenantContext,
    IGroupRepository repository,
    ILevelRepository levelRepository
) : IQueryHandler<GetSuggestedGroupCodeQuery, string>
{
    public async ValueTask<string> Handle(
        GetSuggestedGroupCodeQuery query,
        CancellationToken cancellationToken
    )
    {
        var level =
            await levelRepository.GetByIdAsync(query.LevelId, cancellationToken)
            ?? throw new NotFoundException($"Уровень {query.LevelId} не найден.");

        var codes = await repository.GetCodesByOrganizationAsync(
            tenantContext.OrganizationId,
            cancellationToken
        );

        var levelCode = level.Code.Value;

        // Ищем коды с паттерном {LEVEL}-{N} или {LEVEL}-{N} в любом месте
        var maxNumber = codes
            .Where(c => c.StartsWith($"{levelCode}-", StringComparison.OrdinalIgnoreCase))
            .Select(c =>
            {
                var suffix = c[(levelCode.Length + 1)..];
                // Берём только последний числовой сегмент (напр. EN-B1-12 → "12")
                var lastSegment = suffix.Split('-').LastOrDefault() ?? string.Empty;
                return int.TryParse(lastSegment, out var n) ? n : 0;
            })
            .DefaultIfEmpty(0)
            .Max();

        var nextNumber = maxNumber + 1;

        // Формат: B1-01, B1-10, B1-99 ...
        return $"{levelCode}-{nextNumber:D2}";
    }
}
