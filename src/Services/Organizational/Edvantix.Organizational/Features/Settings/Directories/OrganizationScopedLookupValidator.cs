using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Features.Settings.Directories;

/// <summary>
/// Базовый валидатор для команд создания/обновления записей справочника
/// (любого наследника <see cref="OrganizationScopedLookup"/>).
/// <para>Проверки:</para>
/// <list type="bullet">
///   <item><c>OrganizationId</c> не пустой (<see cref="OrganizationIdProperty"/>).</item>
///   <item><c>Name</c> не пуст и длина 1..120 после <c>Trim</c> (<see cref="NameProperty"/>).</item>
///   <item>В рамках организации нет другой не архивной записи с тем же именем.</item>
/// </list>
/// <para>Так как наследники работают с разными формами команды, селекторы
/// извлечения <c>OrganizationId</c>, <c>Name</c> и <c>Id</c> передаются через конструктор;
/// сообщения об ошибках привязываются к стабильным именам свойств.</para>
/// </summary>
/// <typeparam name="TCreate">Тип команды (создания или обновления).</typeparam>
public abstract class OrganizationScopedLookupValidator<TCreate> : AbstractValidator<TCreate>
{
    /// <summary>Имя свойства, под которым в <c>ValidationResult</c> привязаны ошибки имени.</summary>
    public const string NameProperty = "Name";

    /// <summary>Имя свойства, под которым в <c>ValidationResult</c> привязаны ошибки <c>OrganizationId</c>.</summary>
    public const string OrganizationIdProperty = "OrganizationId";

    /// <param name="uniqueNameChecker">Проверка уникальности имени по справочнику.</param>
    /// <param name="organizationIdSelector">Делегат, извлекающий <c>OrganizationId</c> из команды.</param>
    /// <param name="nameSelector">Делегат, извлекающий <c>Name</c> из команды.</param>
    /// <param name="excludeIdSelector">
    /// Делегат, возвращающий <c>Id</c> исключаемой записи (для update-сценария);
    /// <c>null</c> — при создании. По умолчанию всегда возвращает <c>null</c> (create).
    /// </param>
    protected OrganizationScopedLookupValidator(
        IUniqueNameChecker uniqueNameChecker,
        Func<TCreate, Guid> organizationIdSelector,
        Func<TCreate, string?> nameSelector,
        Func<TCreate, Guid?>? excludeIdSelector = null
    )
    {
        ArgumentNullException.ThrowIfNull(uniqueNameChecker);
        ArgumentNullException.ThrowIfNull(organizationIdSelector);
        ArgumentNullException.ThrowIfNull(nameSelector);

        var resolveExclude = excludeIdSelector ?? (_ => (Guid?)null);

        RuleFor(command => command)
            .Custom(
                (command, context) =>
                {
                    if (organizationIdSelector(command) == Guid.Empty)
                        context.AddFailure(
                            OrganizationIdProperty,
                            "Идентификатор организации обязателен."
                        );

                    var name = nameSelector(command);
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        context.AddFailure(
                            NameProperty,
                            "Имя записи справочника не может быть пустым."
                        );
                        return;
                    }

                    var length = name.Trim().Length;
                    if (
                        length
                        is < OrganizationScopedLookup.MinNameLength
                            or > OrganizationScopedLookup.MaxNameLength
                    )
                        context.AddFailure(
                            NameProperty,
                            $"Имя записи справочника должно быть от {OrganizationScopedLookup.MinNameLength}"
                                + $" до {OrganizationScopedLookup.MaxNameLength} символов."
                        );
                }
            );

        RuleFor(command => command)
            .CustomAsync(
                async (command, context, ct) =>
                {
                    var orgId = organizationIdSelector(command);
                    var name = nameSelector(command);
                    if (orgId == Guid.Empty || string.IsNullOrWhiteSpace(name))
                        return;

                    var length = name.Trim().Length;
                    if (
                        length
                        is < OrganizationScopedLookup.MinNameLength
                            or > OrganizationScopedLookup.MaxNameLength
                    )
                        return;

                    var exists = await uniqueNameChecker
                        .ExistsAsync(orgId, name.Trim(), resolveExclude(command), ct)
                        .ConfigureAwait(false);

                    if (exists)
                        context.AddFailure(
                            NameProperty,
                            $"Запись с таким именем уже существует в справочнике '{uniqueNameChecker.DirectoryCode}'."
                        );
                }
            );
    }
}
