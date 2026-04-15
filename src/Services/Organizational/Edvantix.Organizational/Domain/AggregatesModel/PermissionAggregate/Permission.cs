using Edvantix.Chassis.Utilities.Guards;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
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

    internal IReadOnlyList<GroupRole> GroupRoles => _groupRoles;
    internal IReadOnlyList<OrganizationMemberRole> OrganizationMemberRoles =>
        _organizationMemberRoles;

    private readonly List<GroupRole> _groupRoles = [];
    private readonly List<OrganizationMemberRole> _organizationMemberRoles = [];
}
