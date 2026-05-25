using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Groups.SuggestCode;

/// <summary>
/// Предлагает уникальный код группы на основе переданного кода уровня.
/// <para>
/// Клиент передаёт <see cref="LevelCode"/> напрямую: уровни теперь хранятся
/// в Organizational-сервисе (cross-service), и Groups-сервис не имеет локального доступа к ним.
/// </para>
/// </summary>
[RequirePermission(GroupPermissions.View)]
public sealed record GetSuggestedGroupCodeQuery(string LevelCode) : IQuery<string>;

internal sealed class GetSuggestedGroupCodeQueryHandler(
    ITenantContext tenantContext,
    IGroupRepository repository
) : IQueryHandler<GetSuggestedGroupCodeQuery, string>
{
    public async ValueTask<string> Handle(
        GetSuggestedGroupCodeQuery query,
        CancellationToken cancellationToken
    )
    {
        var levelCode = query.LevelCode.Trim().ToUpperInvariant();

        var codes = await repository.GetCodesByOrganizationAsync(
            tenantContext.OrganizationId,
            cancellationToken
        );

        // Ищем коды с паттерном {LEVEL}-{N} и находим максимальный порядковый номер
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
