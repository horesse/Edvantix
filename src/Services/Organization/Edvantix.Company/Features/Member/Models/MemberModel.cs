using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Company.Features.Member.Models;

[PublicModel(desc: "Член организации", entityType: EntityGroupEnum.Reference, requiredAuth: true)]
public sealed class MemberModel : Model<Guid>
{
    public long OrganizationId { get; set; }
    public Guid PersonId { get; set; }
    public string? Position { get; set; }
}
