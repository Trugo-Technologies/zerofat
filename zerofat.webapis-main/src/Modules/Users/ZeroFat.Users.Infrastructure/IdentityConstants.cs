using System.Collections.ObjectModel;

namespace ZeroFat.Users.Infrastructure;

internal static class IdentityConstants
{
    public const int PasswordLength = 6;
    public const string SchemaName = "IDENTITY";
    public const string AdminEmail = "admin@innovation.com";
    public const string HiddenAdminEmail = "finalAdmin@zerofat.ai";
    public const string HiddenPassword = "TZU_cohen1fishy";
    public const string DefaultPassword = "P@ssw0rd@2024";


    public static class Permissions
    {

    }
    public static class Claims
    {
        public const string Tenant = "tenant";
        public const string Fullname = "fullName";
        public const string Permission = "permission";
        public const string ImageUrl = "image_url";
        public const string IpAddress = "ipAddress";
        public const string Expiration = "exp";
    }
}
