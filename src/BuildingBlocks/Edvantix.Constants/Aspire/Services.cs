namespace Edvantix.Constants.Aspire;

public static class Services
{
    public static readonly string Gateway = nameof(Gateway).ToLowerInvariant();

    // TODO: Баг. Organizational (с z) - выдает ошибку по scope keycloak
    public static readonly string Organisational = nameof(Organisational).ToLowerInvariant();
    public static readonly string Persona = nameof(Persona).ToLowerInvariant();
    public static readonly string Notification = nameof(Notification).ToLowerInvariant();
    public static readonly string Identity = nameof(Identity).ToLowerInvariant();
    public static readonly string Scheduler = nameof(Scheduler).ToLowerInvariant();

    public static string ToClientName(string application, string? suffix = null)
    {
        var clientName = char.ToUpperInvariant(application[0]) + application[1..];
        return string.IsNullOrWhiteSpace(suffix) ? clientName : $"{clientName} {suffix}";
    }
}
