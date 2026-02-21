using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.GroupFeature.Features.UpdateGroup;

public sealed record UpdateGroupCommand(long Id, string Name, string? Description) : IRequest<Unit>;

public sealed class UpdateGroupCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateGroupCommand, Unit>
{
    public async Task<Unit> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireGroupManagementAsync(request.Id, cancellationToken);

        using var groupRepo = provider.GetRequiredService<IGroupRepository>();
        var group = await groupRepo.GetByIdAsync(request.Id, cancellationToken);

        if (group is null)
            throw new NotFoundException($"Группа с ID {request.Id} не найдена.");

        group.UpdateName(request.Name);
        group.UpdateDescription(request.Description);

        await groupRepo.UpdateAsync(group, cancellationToken);
        await groupRepo.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
