using MessagePack;

namespace EIV_Common.LobbyPackets.Connections.Response;

[MessagePackObject]
public class JoiningResponse : IConnectionResponse
{
    [Key(0)]
    public ConnectAction ConnectAction => ConnectAction.Joining;

    [Key(1)]
    public byte[] Items { get; set; } = [];

    // set player stats and stuff.
}
