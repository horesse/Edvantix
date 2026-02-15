using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Company.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Группа в рамках организации (класс, курс, подразделение).
/// </summary>
public sealed class Group() : LongIdentity, IAggregateRoot, ISoftDelete
{
    private readonly List<GroupMember> _members = [];

    public Group(long organizationId, string name, string? description = null)
        : this()
    {
        if (organizationId <= 0)
            throw new ArgumentException(
                "Некорректный идентификатор организации.",
                nameof(organizationId)
            );

        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        OrganizationId = organizationId;
        Name = name;
        Description = description;
        IsDeleted = false;
    }

    public long OrganizationId { get; private set; }
    public OrganizationAggregate.Organization Organization { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsDeleted { get; set; }

    public IReadOnlyCollection<GroupMember> Members => _members.AsReadOnly();

    /// <summary>
    /// Обновляет название группы.
    /// </summary>
    public void UpdateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        Name = name;
    }

    /// <summary>
    /// Обновляет описание группы.
    /// </summary>
    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Группа уже удалена.");

        IsDeleted = true;

        foreach (var member in _members.Where(m => !m.IsDeleted))
            member.Delete();
    }
}
