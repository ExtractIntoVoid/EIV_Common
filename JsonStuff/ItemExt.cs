using EIV_JsonLib.Interfaces;

namespace EIV_Common.JsonStuff;

public static class ItemExt
{
    public static bool Is<T>(this IItem item) where T : IItem
    {
        return item is T;
    }

    public static T? As<T>(this IItem item) where T : IItem
    {
        if (item == null)
            return default;
        if (item.Is<T>())
            return (T)item;
        return default;
    }

    public static bool HasValidAssetPath(this IItem item)
    {
        return Godot.FileAccess.FileExists(item.AssetPath);
    }
}
