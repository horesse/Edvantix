using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.PaymentMethods.Reorder;

/// <summary>Переупорядочить способы оплаты согласно переданному списку идентификаторов.</summary>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record ReorderPaymentMethodsCommand(IReadOnlyList<Guid> OrderedIds) : ICommand;

internal sealed class ReorderPaymentMethodsCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IPaymentMethodRepository repository
) : ICommandHandler<ReorderPaymentMethodsCommand>
{
    public async ValueTask<Unit> Handle(
        ReorderPaymentMethodsCommand command,
        CancellationToken cancellationToken
    )
    {
        var items = await repository.ListAsync(
            new PaymentMethodReorderSpec(tenantContext.OrganizationId),
            cancellationToken
        );

        var lookup = items.ToDictionary(pm => pm.Id);
        var by = claims.GetProfileIdOrError();

        for (var i = 0; i < command.OrderedIds.Count; i++)
        {
            if (lookup.TryGetValue(command.OrderedIds[i], out var item))
                item.SetOrder(i, by);
        }

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
