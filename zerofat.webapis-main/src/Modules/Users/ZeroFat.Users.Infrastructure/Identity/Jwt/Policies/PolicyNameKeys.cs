namespace ZeroFat.Users.Infrastructure.Auth.Jwt;

public static class PolicyNameKeys
{
    public const string TokenValid = "TokenValid";
    public const string HasPermission = "HasPermission";
    public const string SameUser = "SameUser";
    public const string NotSameUser = "NotSameUser";
}
