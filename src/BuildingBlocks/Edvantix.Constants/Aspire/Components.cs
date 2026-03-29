namespace Edvantix.Constants.Aspire;

public static class Components
{
    public const string K6 = "k6";
    public const string Broker = "broker";
    public const string Redis = "redis";
    public const string MailPit = "mailpit";
    public const string Postgres = "postgres";
    public const string KeyCloak = "keycloak";
    public const string Inspector = "inspector";
    public const string VectorDb = "vectordb";
    public const string TurboRepo = "turborepo";
    public const string ContainerRegistry = "container-registry";

    public static class Database
    {
        private const string Suffix = "db";

        public static readonly string Persona = $"{nameof(Persona).ToLowerInvariant()}{Suffix}";

        public static readonly string Notification =
            $"{nameof(Notification).ToLowerInvariant()}{Suffix}";

        public static readonly string Organizational =
            $"{nameof(Organizational).ToLowerInvariant()}{Suffix}";
    }

    public static class Azure
    {
        public const string ContainerApp = "aca";

        public static class Storage
        {
            public const string Resource = "storage";

            public static string BlobContainer(string containerName) => $"{containerName}-blob";
        }
    }
}
