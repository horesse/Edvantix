using Edvantix.Organizational.Domain.AggregatesModel.LegalFormAggregate;
using Edvantix.Organizational.Features.LegalFormFeature.Models;
using Edvantix.Organizational.Features.OrganizationFeature.Models;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.OrganizationFeature.Features.GetOrganization;

public sealed record GetOrganizationQuery(Guid Id) : IQuery<OrganizationModel>;

public sealed class GetOrganizationQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetOrganizationQuery, OrganizationModel>
{
    public async ValueTask<OrganizationModel> Handle(
        GetOrganizationQuery request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.GetCurrentMemberAsync(request.Id, cancellationToken);

        var orgRepo = provider.GetRequiredService<IOrganizationRepository>();
        var org =
            await orgRepo.FindByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Организация с ID {request.Id} не найдена.");

        // TODO: Fetch user profile data from Profile service via gRPC

        var legalFormRepo = provider.GetRequiredService<ILegalFormRepository>();
        var legalForm =
            await legalFormRepo.FindByIdAsync(org.LegalFormId, cancellationToken)
            ?? throw new NotFoundException(
                $"Организационно-правовая форма с ID {org.LegalFormId} не найдена."
            );

        var legalFormMapper = provider.GetRequiredService<IMapper<LegalForm, LegalFormModel>>();

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
            OrganizationType = org.OrganizationType,
            LegalForm = legalFormMapper.Map(legalForm),
        };
    }
}
