using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.OrganizationManagement.Features.Usage.Models;

[PublicModel(desc: "Использование лимита", entityType: EntityGroupEnum.Reference, requiredAuth: true)]
public sealed class UsageModel : Model<long>
{
    public long OrganizationId { get; set; }
    public long LimitId { get; set; }
    public decimal Value { get; set; }
}

