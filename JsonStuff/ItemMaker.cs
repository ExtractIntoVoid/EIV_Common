using EIV_Common.JsonStuff;
using EIV_JsonLib.Interfaces;

namespace EIV_Common.ItemStuff
{
    public class ItemMaker
    {
        public static IItem? MakeNewItem(string BaseId)
        {
            if (!Storage.Items.TryGetValue(BaseId, out IItem? item))
                return null;
            if (item == null)
                return null;
            return (IItem)item.Clone();
        }

        public static T? CreateItem<T>(string BaseId) where T : IItem
        {
            if (!Storage.Items.TryGetValue(BaseId, out IItem? item))
                return default;
            if (item == null)
                return default;

            if (item is T)
            {
                Console.WriteLine(item.ToString());
                return (T)item.Clone();
            }
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
}
