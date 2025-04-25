using EIV_JsonLib.Base;
using GD = Godot;

namespace EIV_Common.Godot.Extensions;

public static partial class ItemExt
{
    public static bool HasValidAssetPath(this CoreItem item)
    {
        return GD.FileAccess.FileExists(item.AssetPath) || GD.ResourceLoader.Exists(item.AssetPath);
    }
}
