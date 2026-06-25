using System.Collections.ObjectModel;

namespace ZeroFat.Shared.Authorization;
public static class ZeroFatRoles
{
    public const string Admin = nameof(Admin);
    public const string Client = nameof(Client);
    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(
    [
        Admin, Client
    ]);
}

public static class ZeroFatProviders
{
    public const string FourDigitPhoneProvider = "4DigitPhone";
    public const string FourDigitEmailProvider = "4DigitEmail";
}

