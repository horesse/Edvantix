using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.GroupFeature.Features.UpdateGroup;

public sealed record UpdateGroupCommand(Guid Id, string Name, string? Description) : IRequest<Unit>;

public sealed class UpdateGroupCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateGroupCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        UpdateGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireGroupManagementAsync(request.Id, cancellationToken);

        var groupRepo = provider.GetRequiredService<IGroupRepository>();
        var group =
            await groupRepo.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Группа с ID {request.Id} не найдена.");

        group.UpdateName(request.Name);
        group.UpdateDescription(request.Description);

        await groupRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
