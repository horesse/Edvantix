using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.PaymentMethods.Update;

/// <summary>Запрос на обновление способа оплаты.</summary>
/// <param name="Id">Идентификатор записи (из маршрута).</param>
/// <param name="Name">Новое название.</param>
/// <param name="Code">Новый машинный код (до 20 символов).</param>
/// <param name="IsCashless">Новый признак безналичного расчёта.</param>
/// <param name="RequiresContract">Новый признак необходимости договора.</param>
/// <param name="Order">Новый порядок сортировки.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record UpdatePaymentMethodCommand(
    Guid Id,
    string Name,
    string Code,
    bool IsCashless,
    bool RequiresContract,
    int Order = 0
) : ICommand<PaymentMethodDto>;

internal sealed class UpdatePaymentMethodCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IPaymentMethodRepository repository,
    IMapper<PaymentMethod, PaymentMethodDto> mapper
) : ICommandHandler<UpdatePaymentMethodCommand, PaymentMethodDto>
{
    public async ValueTask<PaymentMethodDto> Handle(
        UpdatePaymentMethodCommand command,
        CancellationToken cancellationToken
    )
    {
        var paymentMethod = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (paymentMethod is null || paymentMethod.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<PaymentMethod>(command.Id);

        var modifiedBy = claims.GetProfileIdOrError();

        paymentMethod.Update(
            command.Name,
            command.Code,
            command.IsCashless,
            command.RequiresContract,
            command.Order,
            modifiedBy
        );

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(paymentMethod);
    }
}
