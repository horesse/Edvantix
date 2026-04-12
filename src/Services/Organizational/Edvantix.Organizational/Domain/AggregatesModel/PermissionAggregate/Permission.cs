using Edvantix.Chassis.Utilities.Guards;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

public sealed class Permission() : Entity, IAggregateRoot
{
    public Permission(string feature, string name)
        : this()
    {
        Guard.Against.NullOrWhiteSpace(feature, nameof(feature));
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Feature = feature.Trim();
        Name = name.Trim();
    }

    public string Feature { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
}
