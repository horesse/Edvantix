using Edvantix.Chassis.Exceptions;
using Edvantix.Company.Domain.AggregatesModel.ContactAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Services;
using MediatR;

namespace Edvantix.Company.Features.ContactFeature.Features.DeleteContact;

public sealed record DeleteContactCommand(long OrganizationId, long ContactId) : IRequest<Unit>;

public sealed class DeleteContactCommandHandler(IServiceProvider provider)
    : IRequestHandler<DeleteContactCommand, Unit>
{
    public async Task<Unit> Handle(
        DeleteContactCommand request,
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

        await contactRepo.DeleteAsync(contact, cancellationToken);
        await contactRepo.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
