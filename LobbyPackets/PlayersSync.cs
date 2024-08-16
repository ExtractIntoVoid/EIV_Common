using MessagePack;

namespace EIV_Common.LobbyPackets;


[MessagePackObject]
public class PlayersSync
{
    [Key(0)]
    public List<string /* UserId */> PlayerUserIds { get; set; } = [];

}
