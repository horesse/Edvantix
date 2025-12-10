namespace Edvantix.Constants.Aspire;

public static class Components
{
    public static readonly string Queue = nameof(Queue).ToLowerInvariant();
    public static readonly string Redis = nameof(Redis).ToLowerInvariant();
    public static readonly string MailPit = nameof(MailPit).ToLowerInvariant();
    public static readonly string Postgres = nameof(Postgres).ToLowerInvariant();
    public static readonly string KeyCloak = nameof(KeyCloak).ToLowerInvariant();
    public static readonly string Inspector = nameof(Inspector).ToLowerInvariant();
    public static readonly string VectorDb = nameof(VectorDb).ToLowerInvariant();

    public static class Database
    {
        private const string Suffix = "db";

        public static readonly string DataVault = $"{nameof(DataVault).ToLowerInvariant()}{Suffix}";
    }

    public static class Azure
    {
        public const string ContainerApp = "aca";

        public static class Storage
        {
            public static readonly string Resource = nameof(Storage).ToLowerInvariant();
            public static readonly string Blob = nameof(Blob).ToLowerInvariant();
        }
    }
}
