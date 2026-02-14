using Edvantix.Company.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Grpc.Services;
using MediatR;

namespace Edvantix.Company.Features.OrganizationFeature.Features.CreateOrganization;

public sealed record CreateOrganizationCommand(
    string Name,
    string NameLatin,
    string ShortName,
    string? PrintName,
    string? Description
) : IRequest<long>;

public sealed class CreateOrganizationCommandHandler(IServiceProvider provider)
    : IRequestHandler<CreateOrganizationCommand, long>
{
    public async Task<long> Handle(
        CreateOrganizationCommand request,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        using var orgRepo = provider.GetRequiredService<IOrganizationRepository>();

        await using var transaction = await orgRepo.BeginTransactionAsync(cancellationToken);

        try
        {
            var organization = new Organization(
                request.Name,
                request.NameLatin,
                request.ShortName,
                DateTime.UtcNow,
                request.PrintName,
                request.Description
            );

            await orgRepo.InsertAsync(organization, cancellationToken);
            await orgRepo.SaveEntitiesAsync(cancellationToken);

            // Создатель становится Owner
            using var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();
            var member = new OrganizationMember(
                organization.Id,
                profileId,
                OrganizationRole.Owner
            );

            await memberRepo.InsertAsync(member, cancellationToken);
            await memberRepo.SaveEntitiesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return organization.Id;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
