using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Edvantix.Chassis.Caching;

[OptionsValidator]
public sealed partial class CachingOptions : IValidateOptions<CachingOptions>
{
    public const string ConfigurationSection = "Caching";

    [Required]
    public TimeSpan Expiration { get; set; }
}
