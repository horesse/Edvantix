using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.PaymentMethods;

/// <summary>
/// Реализация <see cref="IUniqueNameChecker"/> для справочника «Способы оплаты».
/// Делегирует проверку в <see cref="IPaymentMethodRepository"/> через спецификацию.
/// </summary>
internal sealed class PaymentMethodUniqueNameChecker(IPaymentMethodRepository repository)
    : IUniqueNameChecker
{
    /// <inheritdoc/>
    public string DirectoryCode => DirectoryCatalog.PaymentMethods;

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(
        Guid organizationId,
        string name,
        Guid? excludeId,
        CancellationToken ct
    ) =>
        repository.AnyAsync(
            new PaymentMethodUniqueNameSpecification(organizationId, name, excludeId),
            ct
        );
}
