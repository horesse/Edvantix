using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.DataVault.Features.PlaygroundEntityFeature.Models;

[PublicModel("Игровая сущность", true)]
public sealed class PlaygroundEntityModel : Model<long>
{
    public string Name { get; set; } = null!;
    public decimal Value { get; set; }
}
