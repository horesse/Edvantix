namespace Edvantix.Organizational.Features.Settings.Directories;

/// <summary>
/// Абстракция для проверки уникальности имени записи справочника
/// в рамках организации среди не архивных записей.
/// <para>Конкретные реализации — по одной на справочник — обращаются к репозиторию своего агрегата.</para>
/// </summary>
public interface IUniqueNameChecker
{
    /// <summary>Код справочника, который обслуживает данная реализация (см. <see cref="DirectoryCatalog"/>).</summary>
    string DirectoryCode { get; }

    /// <summary>
    /// Проверяет, существует ли в указанной организации не архивная запись с данным именем.
    /// </summary>
    /// <param name="organizationId">Идентификатор организации (тенанта).</param>
    /// <param name="name">Имя для проверки (уже <c>Trim</c>-нутое).</param>
    /// <param name="excludeId">Идентификатор записи, исключаемой из проверки (для сценария update).</param>
    /// <param name="ct">Токен отмены.</param>
    Task<bool> ExistsAsync(
        Guid organizationId,
        string name,
        Guid? excludeId,
        CancellationToken ct
    );
}
