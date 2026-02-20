namespace Edvantix.Constants.Aspire;

public static class Clients
{
    private const string Prefix = "@edvantix";

    public const string OrganizationFront = "organizationfront";
    public const string OrganizationTurboApp = $"{Prefix}/{OrganizationFront}";

    public const string BlogFront = "blogfront";
    public const string BlogTurboApp = $"{Prefix}/{BlogFront}";
}
