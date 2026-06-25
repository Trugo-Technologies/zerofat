namespace ZeroFat.Users.Application.Statistics;

public class StatisticsDto
{
    public int Users { get; set; }
    public int AdminUsers { get; set; }
    public int ClientUsers { get; set; }
    public int Devices { get; set; }
    public int TrustedDevices { get; set; }
    public int FailedLoginAttempts { get; set; }
    public Dictionary<string, int> UsersByType { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> DevicesByUser { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> DevicesByType { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> FailedLoginAttemptsByUser { get; set; } = new Dictionary<string, int>();
}
