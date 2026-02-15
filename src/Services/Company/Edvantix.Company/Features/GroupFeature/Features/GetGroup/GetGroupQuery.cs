using Edvantix.Chassis.Exceptions;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Company.Features.GroupFeature.Models;
using Edvantix.Company.Services;
using MediatR;

namespace Edvantix.Company.Features.GroupFeature.Features.GetGroup;

public sealed record GetGroupQuery(long Id) : IRequest<GroupModel>;

public sealed class GetGroupQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetGroupQuery, GroupModel>
{
    public async Task<GroupModel> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        using var groupRepo = provider.GetRequiredService<IGroupRepository>();
        var group = await groupRepo.GetByIdAsync(request.Id, cancellationToken);

        if (group is null)
            throw new NotFoundException($"Группа с ID {request.Id} не найдена.");

        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.GetCurrentMemberAsync(group.OrganizationId, cancellationToken);

        return new GroupModel
        {
            Id = group.Id,
            OrganizationId = group.OrganizationId,
            Name = group.Name,
            Description = group.Description,
            MembersCount = group.Members.Count,
        };
    }
}
