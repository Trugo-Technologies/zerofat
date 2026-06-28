namespace ZeroFat.ClientPortal.Domain.ClientManagement;

public static class ClientAccessBlockHelper
{
    public static bool IsBlocked(ClientBlockOption blockOption, DateTime? blockedUntil, DateTime utcNow)
    {
        if (blockOption == ClientBlockOption.None)
            return false;

        if (blockOption == ClientBlockOption.Permanent)
            return true;

        return blockedUntil.HasValue && blockedUntil.Value > utcNow;
    }

    public static bool IsBlocked(Client client, DateTime utcNow)
        => IsBlocked(client.BlockOption, client.BlockedUntil, utcNow);

    public static bool IsExpiredTemporaryBlock(ClientBlockOption blockOption, DateTime? blockedUntil, DateTime utcNow)
        => blockOption is ClientBlockOption.OneDay or ClientBlockOption.Custom
            && blockedUntil.HasValue
            && blockedUntil.Value <= utcNow;

    public static void ClearBlock(Client client)
    {
        client.BlockOption = ClientBlockOption.None;
        client.BlockedUntil = null;
        client.BlockedOn = null;
    }

    public static string GetBlockDescription(ClientBlockOption option, DateTime? blockedUntil)
        => option switch
        {
            ClientBlockOption.OneDay => "Blocked for 1 day",
            ClientBlockOption.Custom when blockedUntil.HasValue =>
                $"Blocked until {blockedUntil.Value:yyyy-MM-dd HH:mm} UTC",
            ClientBlockOption.Permanent => "Permanently blocked",
            _ => "Active"
        };
}
