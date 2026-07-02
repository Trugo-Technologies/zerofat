namespace ZeroFat.Infrastructure.Email;

/// <summary>SMTP settings — loaded from Configurations/email.json (section EmailSettings).</summary>
public class EmailSettings
{
    public const string SectionName = "EmailSettings";
    public bool Enabled { get; set; }
    public string? FromAddress { get; set; }
    public string? FromName { get; set; }
    public string? SmtpHost { get; set; }
    public int SmtpPort { get; set; } = 587;
    public string? SmtpUsername { get; set; }
    public string? SmtpPassword { get; set; }
    public bool UseSsl { get; set; } = true;
}
