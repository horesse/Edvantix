namespace Edvantix.Constants.Core;

public static class Authorization
{
    public static class Roles
    {
        public static readonly string Admin = nameof(Admin).ToLowerInvariant();
        public static readonly string User = nameof(User).ToLowerInvariant();
        public static readonly string Reporter = nameof(Reporter).ToLowerInvariant();
    }

    public static class Policies
    {
        public const string Admin = nameof(Admin);
        public const string User = nameof(User);
        public const string Reporter = nameof(Reporter);

        /// <summary>
        /// Политика, требующая наличия claim <c>profile_id</c> в токене.
        /// Применяется в сервисах, которые работают с профилями пользователей.
        /// </summary>
        public const string ProfileRequired = nameof(ProfileRequired);
    }

    public static class Actions
    {
        public static readonly string Read = nameof(Read).ToLowerInvariant();
        public static readonly string Write = nameof(Write).ToLowerInvariant();
    }
}
