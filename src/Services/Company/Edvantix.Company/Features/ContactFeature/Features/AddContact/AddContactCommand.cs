using Edvantix.Company.Domain.AggregatesModel.ContactAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Services;
using Edvantix.Constants.Other;
using MediatR;

namespace Edvantix.Company.Features.ContactFeature.Features.AddContact;

public sealed record AddContactCommand(
    long OrganizationId,
    ContactType Type,
    string Value,
    string? Description
) : IRequest<long>;

public sealed class AddContactCommandHandler(IServiceProvider provider)
    : IRequestHandler<AddContactCommand, long>
{
    public async Task<long> Handle(AddContactCommand request, CancellationToken cancellationToken)
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireOrgRoleAsync(
            request.OrganizationId,
            cancellationToken,
            OrganizationRole.Owner,
            OrganizationRole.Manager
        );

        var contact = new Contact(
            request.OrganizationId,
            request.Type,
            request.Value,
            request.Description
        );

        using var contactRepo = provider.GetRequiredService<IContactRepository>();

        await contactRepo.InsertAsync(contact, cancellationToken);
        await contactRepo.SaveEntitiesAsync(cancellationToken);

        return contact.Id;
    }
}
