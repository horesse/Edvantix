using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Groups.SuggestCode;

[RequirePermission(GroupPermissions.View)]
public sealed record SuggestGroupCodeQuery(GroupLevel Level) : IQuery<string>;

internal sealed class SuggestGroupCodeQueryHandler(
    ITenantContext tenantContext,
    IGroupRepository repository
) : IQueryHandler<SuggestGroupCodeQuery, string>
{
    public async ValueTask<string> Handle(
        SuggestGroupCodeQuery query,
        CancellationToken cancellationToken
    )
    {
        var codes = await repository.GetCodesByOrganizationAsync(
            tenantContext.OrganizationId,
            cancellationToken
        );

        var levelCode = query.Level.ToString().ToUpperInvariant();

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
