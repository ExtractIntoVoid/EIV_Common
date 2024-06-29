using EIV_JsonLib.Interfaces;

namespace EIV_Common.JsonStuff
{
    public static class Storage
    {
        public static Dictionary<string, IItem> Items = [];
        public static Dictionary<string, IEffect> Effects = [];
        public static Dictionary<string, IStash> Stashes = [];
    }
}
