namespace Edvantix.Constants.Aspire;

public static class Services
{
    public static readonly string Gateway = nameof(Gateway).ToLowerInvariant();
    public static readonly string Organizational = nameof(Organizational).ToLowerInvariant();
    public static readonly string Persona = nameof(Persona).ToLowerInvariant();
    public static readonly string Subscriptions = nameof(Subscriptions).ToLowerInvariant();
    public static readonly string Blog = nameof(Blog).ToLowerInvariant();
    public static readonly string Notification = nameof(Notification).ToLowerInvariant();
    public static readonly string Catalog = nameof(Catalog).ToLowerInvariant();
    public static readonly string Scheduler = nameof(Scheduler).ToLowerInvariant();
    public static readonly string Organizations = nameof(Organizations).ToLowerInvariant();

    public static string ToClientName(string application, string? suffix = null)
    {
        var clientName = char.ToUpperInvariant(application[0]) + application[1..];
        return string.IsNullOrWhiteSpace(suffix) ? clientName : $"{clientName} {suffix}";
    }
}
