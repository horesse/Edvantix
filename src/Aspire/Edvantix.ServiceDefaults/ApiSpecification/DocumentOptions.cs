namespace Edvantix.ServiceDefaults.ApiSpecification;

public sealed class DocumentOptions
{
    public const string ConfigurationSection = "Document";
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string AuthorName { get; init; } = "Edvantix";
    public string AuthorEmail { get; init; } = "deepheath322@gmail.com";
    public Uri AuthorUrl { get; init; } = new("https://github.com/edvantix");
    public string LicenseName { get; init; } = "MIT";
    public Uri LicenseUrl { get; init; } = new("https://opensource.org/licenses/MIT");
}
