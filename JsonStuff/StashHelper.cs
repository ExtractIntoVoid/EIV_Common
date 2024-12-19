using EIV_JsonLib;
using EIV_JsonLib.Base;

namespace EIV_Common.JsonStuff;

public static class StashHelper
{
    public static bool CanAddItem<T>(this Stash stash, T item) where T : CoreItem
    {
        if (stash.IsFull())
            return false;
        if (stash.GetCurrentVolume() + item.Volume >= stash.MaxVolume)
            return false;
        if (stash.GetCurrentWeight() + item.Weight >= stash.MaxWeight)
            return false;
        return true;
    }

    public static bool IsFull(this Stash stash)
    {
        return (stash.Items.Count == stash.MaxSize) || (GetCurrentVolume(stash) >= stash.MaxVolume) && (GetCurrentWeight(stash) >= stash.MaxWeight);
    }

    public static decimal GetCurrentWeight(this Stash stash)
    {
        if (stash.Items.Count == 0)
            return 0;
        return stash.Items.Select(x=>x.Weight).Sum();
    }

    public static decimal GetCurrentVolume(this Stash stash)
    {
        if (stash.Items.Count == 0)
            return 0;
        return stash.Items.Select(x => x.Volume).Sum();
    }
}
