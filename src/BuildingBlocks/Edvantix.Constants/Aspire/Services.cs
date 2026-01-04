namespace Edvantix.Constants.Aspire;

public static class Services
{
    public static readonly string Gateway = nameof(Gateway).ToLowerInvariant();
    public static readonly string DataVault = nameof(DataVault).ToLowerInvariant();
    public static readonly string EntityHub = nameof(EntityHub).ToLowerInvariant();
    public static readonly string EntityHubWorker = nameof(EntityHubWorker).ToLowerInvariant();
    public static readonly string Company = nameof(Company).ToLowerInvariant();
    public static readonly string System = nameof(System).ToLowerInvariant();
    public static readonly string Person = nameof(Person).ToLowerInvariant();

    public static string ToClientName(string application, string? suffix = null)
    {
        var clientName = char.ToUpperInvariant(application[0]) + application[1..];
        return string.IsNullOrWhiteSpace(suffix) ? clientName : $"{clientName} {suffix}";
    }
}
