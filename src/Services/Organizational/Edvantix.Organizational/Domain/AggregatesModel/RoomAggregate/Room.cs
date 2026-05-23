using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

/// <summary>
/// Кабинет (помещение) организации — запись справочника «Кабинеты».
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item><see cref="Capacity"/> — от 1 до 1000 мест.</item>
///   <item><see cref="Floor"/> — не более 10 символов после <c>Trim</c>; <c>null</c> допустим.</item>
///   <item>Уникальность <see cref="OrganizationScopedLookup.Name"/> в рамках организации среди не архивных записей.</item>
/// </list>
/// </summary>
public sealed class Room : OrganizationScopedLookup
{
    /// <summary>Минимальная вместимость кабинета.</summary>
    public const int MinCapacity = 1;

    /// <summary>Максимальная вместимость кабинета.</summary>
    public const int MaxCapacity = 1000;

    /// <summary>Максимальная длина строки номера/названия этажа.</summary>
    public const int MaxFloorLength = 10;

    /// <summary>Конструктор для EF Core / десериализации.</summary>
    private Room() { }

    /// <param name="organizationId">Идентификатор организации-владельца.</param>
    /// <param name="name">Отображаемое название кабинета.</param>
    /// <param name="capacity">Вместимость (1–1000).</param>
    /// <param name="floor">Номер/название этажа (до 10 символов); <c>null</c> — не указан.</param>
    /// <param name="roomType">Тип помещения.</param>
    /// <param name="order">Порядок сортировки.</param>
    /// <param name="createdBy">Идентификатор пользователя, создавшего запись.</param>
    public Room(
        Guid organizationId,
        string name,
        int capacity,
        string? floor,
        RoomType roomType,
        int order = 0,
        Guid? createdBy = null
    )
        : base(organizationId, name, order, createdBy)
    {
        ValidateCapacity(capacity);
        ValidateFloor(floor);

        Capacity = capacity;
        Floor = floor?.Trim();
        RoomType = roomType;
    }

    /// <summary>Количество посадочных мест (1–1000).</summary>
    public int Capacity { get; private set; }

    /// <summary>Номер или название этажа (до 10 символов). Может быть <c>null</c>.</summary>
    public string? Floor { get; private set; }

    /// <summary>Тип помещения.</summary>
    public RoomType RoomType { get; private set; }

    /// <summary>
    /// Обновляет данные кабинета.
    /// </summary>
    /// <param name="name">Новое название.</param>
    /// <param name="capacity">Новая вместимость (1–1000).</param>
    /// <param name="floor">Новый номер/название этажа (до 10 символов).</param>
    /// <param name="roomType">Новый тип помещения.</param>
    /// <param name="order">Новый порядок сортировки.</param>
    /// <param name="by">Идентификатор пользователя, выполняющего операцию.</param>
    public void Update(
        string name,
        int capacity,
        string? floor,
        RoomType roomType,
        int order,
        Guid by
    )
    {
        Rename(name, by);
        ValidateCapacity(capacity);
        ValidateFloor(floor);

        Capacity = capacity;
        Floor = floor?.Trim();
        RoomType = roomType;

        SetOrder(order, by);
    }

    private static void ValidateCapacity(int capacity)
    {
        if (capacity < MinCapacity || capacity > MaxCapacity)
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                $"Вместимость кабинета должна быть от {MinCapacity} до {MaxCapacity} мест."
            );
    }

    private static void ValidateFloor(string? floor)
    {
        if (floor is not null && floor.Trim().Length > MaxFloorLength)
            throw new ArgumentException(
                $"Номер/название этажа не может превышать {MaxFloorLength} символов.",
                nameof(floor)
            );
    }
}
