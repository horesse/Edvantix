using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.PaymentMethods.Create;

/// <summary>Запрос на создание способа оплаты в справочнике организации.</summary>
/// <param name="Name">Название способа оплаты.</param>
/// <param name="Code">Машинный код (до 20 символов, уникален per org).</param>
/// <param name="IsCashless">Признак безналичного расчёта.</param>
/// <param name="RequiresContract">Признак необходимости договора.</param>
/// <param name="Order">Порядок сортировки.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record CreatePaymentMethodCommand(
    string Name,
    string Code,
    bool IsCashless,
    bool RequiresContract,
    int Order = 0
) : ICommand<PaymentMethodDto>;

internal sealed class CreatePaymentMethodCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IPaymentMethodRepository repository,
    IMapper<PaymentMethod, PaymentMethodDto> mapper
) : ICommandHandler<CreatePaymentMethodCommand, PaymentMethodDto>
{
    public async ValueTask<PaymentMethodDto> Handle(
        CreatePaymentMethodCommand command,
        CancellationToken cancellationToken
    )
    {
        var createdBy = claims.GetProfileIdOrError();

        var paymentMethod = new PaymentMethod(
            tenantContext.OrganizationId,
            command.Name,
            command.Code,
            command.IsCashless,
            command.RequiresContract,
            command.Order,
            createdBy
        );

        await repository.AddAsync(paymentMethod, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(paymentMethod);
    }
}
