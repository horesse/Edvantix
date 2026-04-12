using Edvantix.Chassis.Utilities.Guards;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

public sealed class Permission() : Entity, IAggregateRoot
{
    public Permission(string name)
        : this()
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Name = name.Trim();
    }

    public string Name { get; private set; } = string.Empty;
}
