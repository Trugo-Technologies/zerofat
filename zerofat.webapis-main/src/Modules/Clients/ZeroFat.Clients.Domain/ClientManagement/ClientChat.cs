namespace ZeroFat.ClientPortal.Domain.ClientManagement;

public class ClientChat : Entity, IAggregateRoot
{
    public DateTime? ChatDate { get; set; }
    public string? ChatId { get; set; }
    public string? ChatType { get; set; }
    public string? ChannelId { get; set; }
    public string? ChannelName { get; set; }
    public string? BaseDeviceId { get; set; }

    public DefaultIdType? ClientId { get; set; }
    public DefaultIdType? DeviceId { get; set; }
}
