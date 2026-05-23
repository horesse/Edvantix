using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.PaymentMethods.GetById;

/// <summary>Запрос получения способа оплаты по идентификатору.</summary>
/// <param name="Id">Идентификатор записи.</param>
[RequirePermission(OrganizationPermissions.View)]
public sealed record GetPaymentMethodByIdQuery(Guid Id) : IQuery<PaymentMethodDto>;

internal sealed class GetPaymentMethodByIdQueryHandler(
    ITenantContext tenantContext,
    IPaymentMethodRepository repository,
    IMapper<PaymentMethod, PaymentMethodDto> mapper
) : IQueryHandler<GetPaymentMethodByIdQuery, PaymentMethodDto>
{
    public async ValueTask<PaymentMethodDto> Handle(
        GetPaymentMethodByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var paymentMethod = await repository.GetByIdAsync(query.Id, cancellationToken);

        if (paymentMethod is null || paymentMethod.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<PaymentMethod>(query.Id);

        return mapper.Map(paymentMethod);
    }
}
