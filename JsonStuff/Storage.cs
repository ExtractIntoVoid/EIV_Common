using EIV_JsonLib;
using EIV_JsonLib.Base;

namespace EIV_Common.JsonStuff;

// Stores all the stuff
public static class Storage
{
    public static Dictionary<string, ItemBase> Items = [];
    public static Dictionary<string, Effect> Effects = [];
    public static Dictionary<string, Stash> Stashes = [];
    public static Dictionary<string, List<ItemRecreator>> PredefinedCharactersInventory = [];

    public static void ClearAll()
    {
        Items.Clear();
        Effects.Clear();
        Stashes.Clear();
        PredefinedCharactersInventory.Clear();
    }
}
