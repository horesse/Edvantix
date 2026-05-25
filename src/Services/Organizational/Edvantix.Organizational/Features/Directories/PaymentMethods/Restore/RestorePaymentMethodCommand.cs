using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.PaymentMethods.Restore;

/// <summary>Запрос на восстановление способа оплаты из архива.</summary>
/// <param name="Id">Идентификатор записи.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record RestorePaymentMethodCommand(Guid Id) : ICommand;

internal sealed class RestorePaymentMethodCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IPaymentMethodRepository repository
) : ICommandHandler<RestorePaymentMethodCommand>
{
    public async ValueTask<Unit> Handle(
        RestorePaymentMethodCommand command,
        CancellationToken cancellationToken
    )
    {
        var paymentMethod = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (paymentMethod is null || paymentMethod.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<PaymentMethod>(command.Id);

        var by = claims.GetProfileIdOrError();

        paymentMethod.Restore(by);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
