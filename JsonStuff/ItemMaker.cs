using EIV_JsonLib.Base;

namespace EIV_Common.JsonStuff;

public class ItemMaker
{
    public static ItemBase? MakeNewItem(string baseId)
    {
        if (!Storage.Items.TryGetValue(baseId, out ItemBase? item))
            return null;
        return (ItemBase)item.Clone();
    }

    public static T? CreateItem<T>(string baseId) where T : ItemBase
    {
        if (!Storage.Items.TryGetValue(baseId, out ItemBase? item))
            return default;
        if (item is T)
            return (T)item.Clone();
        return default;
    }
    
    public static void PrintBaseIds()
    {
        foreach (var item in Storage.Items)
        {
            Console.WriteLine(item.Key);
        }
    }
}
