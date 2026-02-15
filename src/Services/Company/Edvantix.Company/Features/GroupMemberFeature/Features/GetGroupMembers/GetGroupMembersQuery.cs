using System.ComponentModel;
using Edvantix.Chassis.Exceptions;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate.Specifications;
using Edvantix.Company.Features.GroupMemberFeature.Models;
using Edvantix.Company.Services;
using Edvantix.Constants.Core;
using Edvantix.SharedKernel.Results;
using MediatR;

namespace Edvantix.Company.Features.GroupMemberFeature.Features.GetGroupMembers;

public sealed record GetGroupMembersQuery(
    long GroupId,
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
) : IRequest<PagedResult<GroupMemberModel>>;

public sealed class GetGroupMembersQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetGroupMembersQuery, PagedResult<GroupMemberModel>>
{
    public async Task<PagedResult<GroupMemberModel>> Handle(
        GetGroupMembersQuery request,
        CancellationToken cancellationToken
    )
    {
        // Проверить, что группа существует и получить organizationId
        using var groupRepo = provider.GetRequiredService<IGroupRepository>();
        var group = await groupRepo.GetByIdAsync(request.GroupId, cancellationToken);

        if (group is null)
            throw new NotFoundException($"Группа с ID {request.GroupId} не найдена.");

        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.GetCurrentMemberAsync(group.OrganizationId, cancellationToken);

        var spec = new GroupMemberSpecification { GroupId = request.GroupId };

        using var groupMemberRepo = provider.GetRequiredService<IGroupMemberRepository>();

        var count = await groupMemberRepo.GetCountByExpressionAsync(spec, cancellationToken);

        spec.Skip = (request.PageIndex - 1) * request.PageSize;
        spec.Take = request.PageSize;

        var members = await groupMemberRepo.GetByExpressionAsync(spec, cancellationToken);

        // TODO: Fetch user profile data from Profile service via gRPC
        var items = members
            .Select(m => new GroupMemberModel
            {
                Id = m.Id,
                GroupId = m.GroupId,
                ProfileId = m.ProfileId,
                Role = m.Role,
                JoinedAt = m.JoinedAt,
            })
            .ToList();

        return new PagedResult<GroupMemberModel>(items, request.PageIndex, request.PageSize, count);
    }
}
