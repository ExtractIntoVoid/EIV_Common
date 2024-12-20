using EIV_JsonLib;
using EIV_JsonLib.Base;
using EIV_JsonLib.Profile;
using EIV_JsonLib.Profile.ProfileModules;

namespace EIV_Common.JsonStuff;

// Stores all the stuff
public static class Storage
{
    public static Dictionary<string, CoreItem> Items = [];
    public static Dictionary<string, Effect> Effects = [];
    public static Dictionary<string, Stash> Stashes = [];
    public static Dictionary<string, Inventory> Inventories = [];
    public static Dictionary<string, List<IProfileModule>> OriginToModules = [];

    public static void ClearAll()
    {
        Items.Clear();
        Effects.Clear();
        Stashes.Clear();
        Inventories.Clear();
    }
}
