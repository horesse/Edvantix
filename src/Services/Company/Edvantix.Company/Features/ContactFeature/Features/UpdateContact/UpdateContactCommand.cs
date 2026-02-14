using Edvantix.Chassis.Exceptions;
using Edvantix.Company.Domain.AggregatesModel.ContactAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Services;
using Edvantix.Constants.Other;
using MediatR;

namespace Edvantix.Company.Features.ContactFeature.Features.UpdateContact;

public sealed record UpdateContactCommand(
    long OrganizationId,
    long ContactId,
    ContactType Type,
    string Value,
    string? Description
) : IRequest<Unit>;

public sealed class UpdateContactCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateContactCommand, Unit>
{
    public async Task<Unit> Handle(
        UpdateContactCommand request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.RequireOrgRoleAsync(
            request.OrganizationId,
            cancellationToken,
            OrganizationRole.Owner,
            OrganizationRole.Manager
        );

        using var contactRepo = provider.GetRequiredService<IContactRepository>();
        var contact = await contactRepo.GetByIdAsync(request.ContactId, cancellationToken);

        if (contact is null || contact.OrganizationId != request.OrganizationId)
            throw new NotFoundException($"Контакт с ID {request.ContactId} не найден.");

        contact.UpdateType(request.Type);
        contact.UpdateValue(request.Value);
        contact.UpdateDescription(request.Description);

        await contactRepo.UpdateAsync(contact, cancellationToken);
        await contactRepo.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
