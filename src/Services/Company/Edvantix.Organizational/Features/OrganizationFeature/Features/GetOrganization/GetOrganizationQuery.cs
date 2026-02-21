using Edvantix.Organizational.Features.OrganizationFeature.Models;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.GetOrganization;

public sealed record GetOrganizationQuery(long Id) : IRequest<OrganizationModel>;

public sealed class GetOrganizationQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetOrganizationQuery, OrganizationModel>
{
    public async Task<OrganizationModel> Handle(
        GetOrganizationQuery request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.GetCurrentMemberAsync(request.Id, cancellationToken);

        using var orgRepo = provider.GetRequiredService<IOrganizationRepository>();
        var org = await orgRepo.GetByIdAsync(request.Id, cancellationToken);

        if (org is null)
            throw new NotFoundException($"Организация с ID {request.Id} не найдена.");

        // TODO: Fetch user profile data from Profile service via gRPC

        return new OrganizationModel
        {
            Id = org.Id,
            Name = org.Name,
            NameLatin = org.NameLatin,
            ShortName = org.ShortName,
            PrintName = org.PrintName,
            Description = org.Description,
            RegistrationDate = org.RegistrationDate,
            MembersCount = org.Members.Count,
            GroupsCount = org.Groups.Count,
        };
    }
}
