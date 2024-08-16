using MessagePack;

namespace EIV_Common.LobbyPackets.Connections.Response;

[Union(0, typeof(JoiningResponse))]
public interface IConnectionResponse
{
    public ConnectAction ConnectAction { get; }

}
