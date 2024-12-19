using EIV_JsonLib.Base;

namespace EIV_JsonLib.Extension;

public static partial class ItemExt
{
    public static bool HasValidAssetPath(this CoreItem item)
    {
        return Godot.FileAccess.FileExists(item.AssetPath) || Godot.ResourceLoader.Exists(item.AssetPath);
    }
}
