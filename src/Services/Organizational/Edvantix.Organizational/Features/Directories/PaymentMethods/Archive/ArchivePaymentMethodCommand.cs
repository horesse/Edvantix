using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.PaymentMethods.Archive;

/// <summary>Запрос на архивацию способа оплаты.</summary>
/// <param name="Id">Идентификатор записи.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record ArchivePaymentMethodCommand(Guid Id) : ICommand;

internal sealed class ArchivePaymentMethodCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IPaymentMethodRepository repository
) : ICommandHandler<ArchivePaymentMethodCommand>
{
    public async ValueTask<Unit> Handle(
        ArchivePaymentMethodCommand command,
        CancellationToken cancellationToken
    )
    {
        var paymentMethod = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (paymentMethod is null || paymentMethod.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<PaymentMethod>(command.Id);

        var by = claims.GetProfileIdOrError();

        paymentMethod.Archive(by);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
