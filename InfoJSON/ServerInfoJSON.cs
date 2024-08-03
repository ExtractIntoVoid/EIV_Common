using Newtonsoft.Json;

namespace EIV_Common.InfoJSON;

public class ServerInfoJSON
{
    public ConnectionInfo Connection { get; set; } = new();

    public List<ModInfo> Mods { get; set; } = new();

    public GameInfo Game { get; set; } = new();

    [JsonIgnore]
    public DateTimeOffset LastHeartBeat;

    public class ConnectionInfo
    {
        [JsonRequired]
        public string ServerAddress { get; set; } = string.Empty;

        [JsonRequired]
        public int ServerPort { get; set; } = 0;

        public override string ToString()
        {
            return $"{ServerAddress}:{ServerPort}";
        }
    }

    public class GameInfo
    {
        [JsonRequired]
        public string Version { get; set; } = string.Empty;
        public List<string> Maps { get; set; } = new();
        public int PlayerNumbers { get; set; } = 0;
    }

    public class ModInfo
    {
        public string ModName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
    }
}
