namespace Edvantix.Person.CQRS;

/// <summary>
/// Маркер-интерфейс для команд, которые работают с PersonalData сущностями.
/// Используется в PersonalDataBehavior для автоматического заполнения PersonInfoId.
/// </summary>
public interface IPersonalDataCommand
{
    /// <summary>
    /// ID PersonInfo, к которому относятся персональные данные
    /// </summary>
    long PersonInfoId { get; set; }
}
