using Edvantix.OrganizationManagement.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.MemberAggregate;

public sealed class Member() : Entity<Guid>, IAggregateRoot, ISoftDelete
{
    public Member(long organizationId, Guid personId, string? position = null)
        : this()
    {
        if (organizationId <= 0)
            throw new ArgumentException(
                "Некорректный идентификатор организации.",
                nameof(organizationId)
            );

        if (personId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор пользователя не может быть пустым.",
                nameof(personId)
            );

        OrganizationId = organizationId;
        PersonId = personId;
        Position = position;
        IsDeleted = false;
    }

    public long OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public Guid PersonId { get; private set; }
    public string? Position { get; private set; }
    public bool IsDeleted { get; set; }

    public void UpdatePosition(string? newPosition)
    {
        Position = newPosition;
    }

    public void Delete()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Член организации уже удалён.");

        IsDeleted = true;
    }

    public void Restore()
    {
        if (!IsDeleted)
            throw new InvalidOperationException("Член организации не удалён.");

        IsDeleted = false;
    }
}
