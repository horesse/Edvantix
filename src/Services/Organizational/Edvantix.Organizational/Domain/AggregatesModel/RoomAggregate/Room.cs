using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

/// <summary>
/// Физический кабинет организации, используемый для проведения очных занятий.
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item>Вместимость (<see cref="Seats"/>) должна быть от 1 до 200.</item>
///   <item>Удалённые кабинеты не могут быть назначены группам (фильтрация на уровне запроса).</item>
/// </list>
/// </summary>
public sealed class Room() : Entity, IAggregateRoot, ISoftDelete, ITenanted
{
    private const int MinSeats = 1;
    private const int MaxSeats = 200;

    /// <param name="organizationId">Идентификатор организации-владельца.</param>
    /// <param name="label">Метка кабинета (напр. «Каб. 204», «Зал A»).</param>
    /// <param name="floor">Номер этажа.</param>
    /// <param name="seats">Количество мест (1–200).</param>
    public Room(Guid organizationId, string label, short floor, short seats)
        : this()
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );

        Guard.Against.NullOrWhiteSpace(label, nameof(label));
        ValidateSeats(seats);

        OrganizationId = organizationId;
        Label = label.Trim();
        Floor = floor;
        Seats = seats;
        IsDeleted = false;
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>Метка кабинета, отображаемая в интерфейсе.</summary>
    public string Label { get; private set; } = string.Empty;

    /// <summary>Номер этажа.</summary>
    public short Floor { get; private set; }

    /// <summary>Количество посадочных мест (1–200).</summary>
    public short Seats { get; private set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Обновляет данные кабинета.
    /// </summary>
    /// <param name="label">Новая метка.</param>
    /// <param name="floor">Номер этажа.</param>
    /// <param name="seats">Новое количество мест.</param>
    /// <exception cref="ArgumentException">Если метка пуста или вместимость вне диапазона.</exception>
    public void Update(string label, short floor, short seats)
    {
        Guard.Against.NullOrWhiteSpace(label, nameof(label));
        ValidateSeats(seats);

        Label = label.Trim();
        Floor = floor;
        Seats = seats;
    }

    /// <summary>
    /// Изменяет вместимость кабинета.
    /// </summary>
    /// <param name="seats">Новое количество мест (1–200).</param>
    /// <exception cref="ArgumentOutOfRangeException">Если значение вне диапазона.</exception>
    public void Resize(short seats)
    {
        ValidateSeats(seats);
        Seats = seats;
    }

    /// <inheritdoc />
    public void Delete() => IsDeleted = true;

    private static void ValidateSeats(short seats)
    {
        if (seats < MinSeats || seats > MaxSeats)
            throw new ArgumentOutOfRangeException(
                nameof(seats),
                $"Вместимость кабинета должна быть от {MinSeats} до {MaxSeats} мест."
            );
    }
}
