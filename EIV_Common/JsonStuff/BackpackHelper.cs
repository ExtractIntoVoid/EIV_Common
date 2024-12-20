using EIV_JsonLib.Base;
using EIV_JsonLib;

namespace EIV_Common.JsonStuff;

public static class BackpackHelper
{
    public static bool CanAddItem(this Backpack backpack, string itemId)
    {
        var item = ItemMaker.MakeNewItem(itemId);
        if (item == null)
            return false;
        return backpack.CanAddItem(item);
    }

    public static bool CanAddItem<T>(this Backpack backpack, T item) where T : CoreItem
    {
        if (item == null)
            return false;
        if (backpack.IsFull())
            return false;
        if (backpack.GetCurrentVolume() + item.Volume >= backpack.MaxVolume)
            return false;
        if (backpack.GetCurrentWeight() + item.Weight >= backpack.MaxWeight)
            return false;
        return true;
    }

    public static bool IsFull(this Backpack backpack)
    {
        return (backpack.Items.Count == backpack.MaxSize) ||  (backpack.GetCurrentVolume() >= backpack.MaxVolume) && (backpack.GetCurrentWeight() >= backpack.MaxWeight);
    }

    public static decimal GetCurrentWeight(this Backpack backpack)
    {
        if (backpack.Items.Count == 0)
            return 0;
        return backpack.Items.Select(x => x.Weight).Sum();
    }

    public static decimal GetCurrentVolume(this Backpack backpack)
    {
        if (backpack.Items.Count == 0)
            return 0;
        return backpack.Items.Select(x => x.Volume).Sum();
    }
}
