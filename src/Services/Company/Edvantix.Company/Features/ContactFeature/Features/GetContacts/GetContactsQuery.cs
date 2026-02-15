using System.ComponentModel;
using Edvantix.Company.Domain.AggregatesModel.ContactAggregate;
using Edvantix.Company.Domain.AggregatesModel.ContactAggregate.Specifications;
using Edvantix.Company.Features.ContactFeature.Models;
using Edvantix.Company.Services;
using Edvantix.Constants.Core;
using Edvantix.SharedKernel.Results;
using MediatR;

namespace Edvantix.Company.Features.ContactFeature.Features.GetContacts;

public sealed record GetContactsQuery(
    long OrganizationId,
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description(
        "Количество элементов, которые должны быть отображены на одной странице результатов."
    )]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize,
    [property: Description("Свойство для упорядочивания результатов")] string? OrderBy = null,
    [property: Description("При выборе порядка сортировки результат будет в порядке убывания.")]
    [property: DefaultValue(false)]
        bool IsDescending = false
) : IRequest<PagedResult<ContactModel>>;

public sealed class GetContactsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetContactsQuery, PagedResult<ContactModel>>
{
    public async Task<PagedResult<ContactModel>> Handle(
        GetContactsQuery request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.GetCurrentMemberAsync(request.OrganizationId, cancellationToken);

        var spec = new ContactSpecification { OrganizationId = request.OrganizationId };

        using var contactRepo = provider.GetRequiredService<IContactRepository>();

        var count = await contactRepo.GetCountByExpressionAsync(spec, cancellationToken);

        spec.Skip = (request.PageIndex - 1) * request.PageSize;
        spec.Take = request.PageSize;

        var contacts = await contactRepo.GetByExpressionAsync(spec, cancellationToken);

        var items = contacts
            .Select(c => new ContactModel
            {
                Id = c.Id,
                OrganizationId = c.OrganizationId,
                Type = c.Type,
                Value = c.Value,
                Description = c.Description,
            })
            .ToList();

        return new PagedResult<ContactModel>(items, request.PageIndex, request.PageSize, count);
    }
}
