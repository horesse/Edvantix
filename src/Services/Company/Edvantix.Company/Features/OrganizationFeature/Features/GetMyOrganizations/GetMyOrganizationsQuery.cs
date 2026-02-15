using System.ComponentModel;
using Edvantix.Company.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate.Specifications;
using Edvantix.Company.Features.OrganizationFeature.Models;
using Edvantix.Company.Grpc.Services;
using Edvantix.Constants.Core;
using Edvantix.SharedKernel.Results;
using MediatR;

namespace Edvantix.Company.Features.OrganizationFeature.Features.GetMyOrganizations;

public sealed record GetMyOrganizationsQuery(
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
) : IRequest<PagedResult<OrganizationSummaryModel>>;

public sealed class GetMyOrganizationsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetMyOrganizationsQuery, PagedResult<OrganizationSummaryModel>>
{
    public async Task<PagedResult<OrganizationSummaryModel>> Handle(
        GetMyOrganizationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        var spec = new OrganizationMemberByProfileSpecification(profileId);

        using var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();

        var count = await memberRepo.GetCountByExpressionAsync(spec, cancellationToken);

        spec.Skip = (request.PageIndex - 1) * request.PageSize;
        spec.Take = request.PageSize;

        var members = await memberRepo.GetByExpressionAsync(spec, cancellationToken);

        if (members.Count == 0)
            return new PagedResult<OrganizationSummaryModel>(
                [],
                request.PageIndex,
                request.PageSize,
                count
            );

        using var orgRepo = provider.GetRequiredService<IOrganizationRepository>();

        var result = new List<OrganizationSummaryModel>();

        foreach (var member in members)
        {
            var org = await orgRepo.GetByIdAsync(member.OrganizationId, cancellationToken);
            if (org is null)
                continue;

            result.Add(
                new OrganizationSummaryModel(
                    org.Id,
                    org.Name,
                    org.ShortName,
                    org.Description,
                    member.Role.ToString()
                )
            );
        }

        return new PagedResult<OrganizationSummaryModel>(
            result,
            request.PageIndex,
            request.PageSize,
            count
        );
    }
}
