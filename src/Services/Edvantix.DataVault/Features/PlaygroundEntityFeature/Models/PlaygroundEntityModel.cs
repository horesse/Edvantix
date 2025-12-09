using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.DataVault.Features.PlaygroundEntityFeature.Models;

public sealed class PlaygroundEntityModel : Model<long>
{
    public string Name { get; set; } = null!;
    public decimal Value { get; set; }
}
