using EIV_JsonLib.Classes;
using EIV_JsonLib.Interfaces;

namespace EIV_Common.JsonStuff
{
    // Stores all the stuff
    public static class Storage
    {
        public static Dictionary<string, IItem> Items = [];
        public static Dictionary<string, IEffect> Effects = [];
        public static Dictionary<string, IStash> Stashes = [];
        public static Dictionary<string, List<ItemRecreator>> PredefinedCharactersInventory = [];

        public static void ClearAll()
        {
            Items.Clear();
            Effects.Clear();
            Stashes.Clear();
            PredefinedCharactersInventory.Clear();
        }
    }
}
