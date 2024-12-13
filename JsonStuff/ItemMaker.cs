using EIV_JsonLib.Base;

namespace EIV_Common.JsonStuff;

public class ItemMaker
{
    public static CoreItem? MakeNewItem(string baseId)
    {
        if (!Storage.Items.TryGetValue(baseId, out CoreItem? item))
            return null;
        return (CoreItem)item.Clone();
    }

    public static T? CreateItem<T>(string baseId) where T : CoreItem
    {
        if (!Storage.Items.TryGetValue(baseId, out CoreItem? item))
            return default;
        if (item is T)
            return (T)item.Clone();
        return default;
    }

    public static void PrintBaseIds()
    {
        foreach (KeyValuePair<string, CoreItem> item in Storage.Items)
        {
            Console.WriteLine(item.Key);
        }
    }
}
