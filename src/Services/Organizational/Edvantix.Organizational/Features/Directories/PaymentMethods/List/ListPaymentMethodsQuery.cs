using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.PaymentMethods.List;

/// <summary>Запрос постраничного списка способов оплаты организации.</summary>
/// <param name="Search">Строка поиска по названию (опционально).</param>
/// <param name="IncludeArchived">Включать ли архивные записи.</param>
/// <param name="Page">Номер страницы (от 1).</param>
/// <param name="PageSize">Размер страницы.</param>
[RequirePermission(OrganizationPermissions.View)]
public sealed record ListPaymentMethodsQuery(
    [property: Description("Строка поиска по названию")] string? Search = null,
    [property: Description("Включать архивные записи")] bool IncludeArchived = false,
    [property: Description("Номер страницы (от 1)")] int Page = 1,
    [property: Description("Размер страницы")] int PageSize = 50
) : IQuery<PagedResult<PaymentMethodListItemDto>>;

internal sealed class ListPaymentMethodsQueryHandler(
    ITenantContext tenantContext,
    IPaymentMethodRepository repository,
    IMapper<PaymentMethod, PaymentMethodListItemDto> mapper
) : IQueryHandler<ListPaymentMethodsQuery, PagedResult<PaymentMethodListItemDto>>
{
    public async ValueTask<PagedResult<PaymentMethodListItemDto>> Handle(
        ListPaymentMethodsQuery query,
        CancellationToken cancellationToken
    )
    {
        var orgId = tenantContext.OrganizationId;

        // isArchived=null означает «все» (includeArchived=true), false — только активные
        var isArchivedFilter = query.IncludeArchived ? (bool?)null : false;

        var listSpec = new PaymentMethodListSpecification(
            orgId,
            query.IncludeArchived,
            query.Search,
            query.Page,
            query.PageSize
        );

        var countSpec = new PaymentMethodCountSpecification(orgId, isArchivedFilter, query.Search);

        var items = await repository.ListAsync(listSpec, cancellationToken);
        var total = await repository.CountAsync(countSpec, cancellationToken);

        var dtos = mapper.Map(items);

        return new PagedResult<PaymentMethodListItemDto>(
            dtos.ToList(),
            query.Page,
            query.PageSize,
            total
        );
    }
}
