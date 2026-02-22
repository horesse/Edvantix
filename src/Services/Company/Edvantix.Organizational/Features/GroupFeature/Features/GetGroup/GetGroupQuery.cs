using Edvantix.Organizational.Features.GroupFeature.Models;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.GroupFeature.Features.GetGroup;

public sealed record GetGroupQuery(Guid Id) : IRequest<GroupModel>;

public sealed class GetGroupQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetGroupQuery, GroupModel>
{
    public async ValueTask<GroupModel> Handle(
        GetGroupQuery request,
        CancellationToken cancellationToken
    )
    {
        var groupRepo = provider.GetRequiredService<IGroupRepository>();
        var group =
            await groupRepo.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Группа с ID {request.Id} не найдена.");

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
