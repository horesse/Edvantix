using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organization.Features.Org.Models;

[PublicModel(desc: "Организация", entityType: EntityGroupEnum.Reference, requiredAuth: true)]
public sealed class OrganizationModel : Model<long>
{
    public string Name { get; set; } = null!;
    public string NameLatin { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public string? PrintName { get; set; }
    public string? Description { get; set; }
    public DateTime RegistrationDate { get; set; }
}
