namespace Edvantix.Chassis.Security.Tenant;

/// <summary>
/// Scoped-реализация <see cref="ITenantContext"/>.
/// Заполняется с помощью <see cref="TenantMiddleware"/> при каждом входящем запросе.
/// </summary>
public sealed class TenantContext : ITenantContext
{
    private Guid _organizationId;

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Выбрасывается, если происходит доступ до вызова <see cref="Resolve"/>.</exception>
    public Guid OrganizationId =>
        IsResolved
            ? _organizationId
            : throw new InvalidOperationException(
                "Контекст арендатора не определён. Убедитесь, что заголовок X-Organization-Id присутствует."
            );

    /// <inheritdoc/>
    public bool IsResolved { get; private set; }

    /// <inheritdoc/>
    public void Resolve(Guid organizationId)
    {
        _organizationId = organizationId;
        IsResolved = true;
    }
}
