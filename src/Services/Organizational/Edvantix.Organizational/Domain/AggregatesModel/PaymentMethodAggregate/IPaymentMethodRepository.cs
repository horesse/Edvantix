using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;

namespace Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;

/// <summary>Репозиторий агрегата <see cref="PaymentMethod"/>.</summary>
public interface IPaymentMethodRepository : IRepository<PaymentMethod>
{
    /// <summary>Добавляет новый способ оплаты.</summary>
    Task AddAsync(PaymentMethod paymentMethod, CancellationToken ct = default);

    /// <summary>Возвращает способ оплаты по идентификатору (включая архивные).</summary>
    Task<PaymentMethod?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Возвращает список способов оплаты, удовлетворяющих спецификации.</summary>
    Task<IReadOnlyList<PaymentMethod>> ListAsync(
        ISpecification<PaymentMethod> specification,
        CancellationToken ct = default
    );

    /// <summary>Возвращает количество способов оплаты, удовлетворяющих спецификации.</summary>
    Task<int> CountAsync(ISpecification<PaymentMethod> specification, CancellationToken ct = default);

    /// <summary>
    /// Возвращает <see langword="true"/>, если существует хотя бы один способ оплаты,
    /// удовлетворяющий спецификации.
    /// </summary>
    Task<bool> AnyAsync(ISpecification<PaymentMethod> specification, CancellationToken ct = default);

    /// <summary>Дата последнего изменения любой записи организации.</summary>
    Task<DateTime?> GetLastModifiedAtAsync(Guid organizationId, CancellationToken ct = default);
}
