using MessagePack;

namespace EIV_Common.MODAPI.LobbyPackets;


[MessagePackObject()]
public class PlayersSync
{
    public List<string /* UserId */> PlayerUserIds { get; set; } = [];

}
