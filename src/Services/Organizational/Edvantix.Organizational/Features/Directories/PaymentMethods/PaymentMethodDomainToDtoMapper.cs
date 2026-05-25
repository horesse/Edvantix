using Edvantix.Chassis.Mapper;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;

namespace Edvantix.Organizational.Features.Directories.PaymentMethods;

/// <summary>Маппер <see cref="PaymentMethod"/> → <see cref="PaymentMethodDto"/>.</summary>
public sealed class PaymentMethodDtoMapper : Mapper<PaymentMethod, PaymentMethodDto>
{
    /// <inheritdoc/>
    public override PaymentMethodDto Map(PaymentMethod source) =>
        new(
            source.Id,
            source.Name,
            source.Code,
            source.IsCashless,
            source.RequiresContract,
            source.IsArchived,
            source.Order,
            source.OrganizationId,
            source.CreatedAt,
            source.LastModifiedAt,
            source.CreatedBy,
            source.LastModifiedBy
        );
}

/// <summary>Маппер <see cref="PaymentMethod"/> → <see cref="PaymentMethodListItemDto"/>.</summary>
public sealed class PaymentMethodListItemDtoMapper : Mapper<PaymentMethod, PaymentMethodListItemDto>
{
    /// <inheritdoc/>
    public override PaymentMethodListItemDto Map(PaymentMethod source) =>
        new(
            source.Id,
            source.Name,
            source.Code,
            source.IsCashless,
            source.RequiresContract,
            source.IsArchived,
            source.Order
        );
}
