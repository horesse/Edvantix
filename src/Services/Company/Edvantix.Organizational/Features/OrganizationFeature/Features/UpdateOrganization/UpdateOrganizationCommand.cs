using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.UpdateOrganization;

public sealed record UpdateOrganizationCommand(
    long Id,
    string Name,
    string NameLatin,
    string ShortName,
    string? PrintName,
    string? Description
) : IRequest<Unit>;

public sealed class UpdateOrganizationCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateOrganizationCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateOrganizationCommand request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireOrgRoleAsync(
            request.Id,
            cancellationToken,
            OrganizationRole.Owner,
            OrganizationRole.Manager
        );

        using var orgRepo = provider.GetRequiredService<IOrganizationRepository>();
        var org = await orgRepo.GetByIdAsync(request.Id, cancellationToken);

        if (org is null)
            throw new NotFoundException($"Организация с ID {request.Id} не найдена.");

        org.UpdateNames(request.Name, request.NameLatin, request.ShortName, request.PrintName);
        org.UpdateDescription(request.Description);

        await orgRepo.UpdateAsync(org, cancellationToken);
        await orgRepo.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
