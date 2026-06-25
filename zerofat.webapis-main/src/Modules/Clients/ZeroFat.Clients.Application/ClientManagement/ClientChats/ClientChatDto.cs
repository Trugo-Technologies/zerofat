using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientChats;

public class ClientChatSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DateTime? ChatDate { get; set; }
    public string? ChatId { get; set; }
    public string? ChatType { get; set; }
    public string? BaseDeviceId { get; set; }
    public string? ChannelId { get; set; }
    public string? ChannelName { get; set; }

}

public class ClientChatRawDto : ClientChatSimplifyDto
{
    public DefaultIdType? ClientId { get; set; }
    public DefaultIdType? DeviceId { get; set; }
}

public class ClientChatAuditableDto : ClientChatRawDto
{
}

public class ClientChatDto : ClientChatAuditableDto
{
}

public class ClientChatDetailsDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DateTime? ChatDate { get; set; }
    public string? ChatId { get; set; }
    public string? ChatType { get; set; }
    public string? BaseDeviceId { get; set; }
    public string? ChannelId { get; set; }
    public string? ChannelName { get; set; }
    public DefaultIdType? ClientId { get; set; }
    public DefaultIdType? DeviceId { get; set; }
}

