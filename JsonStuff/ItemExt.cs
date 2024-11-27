using EIV_JsonLib.Base;

namespace EIV_Common.JsonStuff;

public static class ItemExt
{
    public static bool Is<T>(this ItemBase item) where T : ItemBase
    {
        return item is T;
    }

    public static T? As<T>(this ItemBase item) where T : ItemBase
    {
        if (item.Is<T>())
            return (T)item;
        return default;
    }

    public static bool HasValidAssetPath(this ItemBase item)
    {
        return Godot.FileAccess.FileExists(item.AssetPath) || Godot.ResourceLoader.Exists(item.AssetPath);
    }
}
