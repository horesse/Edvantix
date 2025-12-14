using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.DataVault.Features.PlaygroundEntityFeature.Models;

[PublicModel(desc: "Игровая сущность", entityType: EntityGroupEnum.Reference, requiredAuth: true)]
public sealed class PlaygroundEntityModel : Model<long>
{
    public string Name { get; set; } = null!;
    public decimal Value { get; set; }
}
