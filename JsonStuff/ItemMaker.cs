using EIV_JsonLib.Interfaces;

namespace EIV_Common.JsonStuff;

public class ItemMaker
{
    public static IItem? MakeNewItem(string baseId)
    {
        if (!Storage.Items.TryGetValue(baseId, out IItem? item))
            return null;
        return (IItem)item.Clone();
    }

    public static T? CreateItem<T>(string baseId) where T : IItem
    {
        if (!Storage.Items.TryGetValue(baseId, out IItem? item))
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
