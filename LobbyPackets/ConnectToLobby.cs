using MessagePack;

namespace EIV_Common.MODAPI.LobbyPackets;

[MessagePackObject()]
public class ConnectToLobby
{
    [Key(0)]
    public string ServerAddress { get; set; } = string.Empty;

    [Key(1)]
    public int ServerPort { get; set; }

    [Key(2)]
    public SemanticVersioning.Version SemVersion { get; set; }

    [Key(3)]
    public string CurrentMapName { get; set; } = string.Empty;

    [Key(4)]
    public MinMax<uint> MinMaxMapPlayers { get; set; } = new(2, 32);


    // Or this should be QueuedPlayers
    [Key(5)]
    public uint CurrentPlayersCount { get; set; }
}
