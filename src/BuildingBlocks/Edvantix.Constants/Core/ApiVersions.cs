using Asp.Versioning;

namespace Edvantix.Constants.Core;

public static class ApiVersions
{
    public static readonly ApiVersion V1 = new(1, 0);
    public static readonly ApiVersion V2 = new(2, 0);
    public static readonly ApiVersion V3 = new(3, 0);
}
