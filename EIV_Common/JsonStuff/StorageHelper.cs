using EIV_JsonLib.Base;
using EIV_JsonLib.Interfaces;

namespace EIV_Common.JsonStuff;

public static class StorageHelper
{
    public static bool CanAddItem<T>(this IStorage storage, T item) where T : CoreItem
    {
        if (storage.IsFull())
            return false;
        if (storage.GetCurrentVolume() + item.Volume >= storage.MaxVolume)
            return false;
        if (storage.GetCurrentWeight() + item.Weight >= storage.MaxWeight)
            return false;
        return true;
    }

    public static bool IsFull(this IStorage storage)
    {
        return (storage.Items.Count == storage.MaxSize) || (storage.GetCurrentVolume() >= storage.MaxVolume) && (storage.GetCurrentWeight() >= storage.MaxWeight);
    }

    public static decimal GetCurrentWeight(this IStorage storage)
    {
        if (storage.Items.Count == 0)
            return 0;
        return storage.Items.Select(x=>x.Weight).Sum();
    }

    public static decimal GetCurrentVolume(this IStorage storage)
    {
        if (storage.Items.Count == 0)
            return 0;
        return storage.Items.Select(x => x.Volume).Sum();
    }
}
