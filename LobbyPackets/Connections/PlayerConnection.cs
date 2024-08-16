using MessagePack;

namespace EIV_Common.LobbyPackets.Connections;

[MessagePackObject]
public class PlayerConnection
{

    [Key(0)]
    public ConnectAction Action { get; set; } = ConnectAction.None;

    [Key(1)]
    public string UserId { get; set; } = string.Empty;

}
