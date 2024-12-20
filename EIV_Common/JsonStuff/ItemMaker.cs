using EIV_JsonLib.Base;
using System.Diagnostics.CodeAnalysis;

namespace EIV_Common.JsonStuff;

public class ItemMaker
{
    /// <summary>
    /// Creating a new Clone of the <see cref="CoreItem"/>
    /// </summary>
    /// <param name="Id">The <see cref="CoreItem"/>'s <see cref="CoreItem.Id"/></param>
    /// <returns>The item clone or null if <paramref name="Id"/> not exists</returns>
    public static CoreItem? MakeNewItem([NotNull] string Id)
    {
        if (!Storage.Items.TryGetValue(Id, out CoreItem? item))
            return null;
        return (CoreItem)item.Clone();
    }

    /// <summary>
    /// Creating a <see cref="CoreItem"/> with desired type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">Any CoreItem</typeparam>
    /// <param name="Id">The <see cref="CoreItem"/>'s <see cref="CoreItem.Id"/> </param>
    /// <returns>Default if not exists, otherwise the copy of <typeparamref name="T"/> </returns>
    public static T? CreateItem<T>([NotNull] string Id) where T : CoreItem
    {
        if (!Storage.Items.TryGetValue(Id, out CoreItem? item))
            return default;
        if (item is T)
            return (T)item.Clone();
        return default;
    }
}
