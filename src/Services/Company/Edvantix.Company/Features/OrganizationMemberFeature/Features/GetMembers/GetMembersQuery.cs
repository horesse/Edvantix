using System.ComponentModel;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate.Specifications;
using Edvantix.Company.Features.OrganizationMemberFeature.Models;
using Edvantix.Company.Services;
using Edvantix.Constants.Core;
using Edvantix.SharedKernel.Results;
using MediatR;

namespace Edvantix.Company.Features.OrganizationMemberFeature.Features.GetMembers;

public sealed record GetMembersQuery(
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
) : IRequest<PagedResult<OrganizationMemberModel>>;

public sealed class GetMembersQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetMembersQuery, PagedResult<OrganizationMemberModel>>
{
    public async Task<PagedResult<OrganizationMemberModel>> Handle(
        GetMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.GetCurrentMemberAsync(request.OrganizationId, cancellationToken);

        var spec = new OrganizationMemberSpecification { OrganizationId = request.OrganizationId };

        using var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();

        var count = await memberRepo.GetCountByExpressionAsync(spec, cancellationToken);

        spec.Skip = (request.PageIndex - 1) * request.PageSize;
        spec.Take = request.PageSize;

        var members = await memberRepo.GetByExpressionAsync(spec, cancellationToken);

        // TODO: Fetch user profile data from Profile service via gRPC
        var items = members
            .Select(m => new OrganizationMemberModel
            {
                Id = m.Id,
                OrganizationId = m.OrganizationId,
                ProfileId = m.ProfileId,
                Role = m.Role,
                JoinedAt = m.JoinedAt,
            })
            .ToList();

        return new PagedResult<OrganizationMemberModel>(
            items,
            request.PageIndex,
            request.PageSize,
            count
        );
    }
}
