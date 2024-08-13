using EIV_JsonLib.Classes;

namespace EIV_Common.JsonStuff;

public static class ItemRecreatorHelper
{
    public static void AddToItemsSlot(this List<ItemRecreator> itemRecreators, List<(string name, uint count)> values)
    {
        foreach (var item in values)
        {
            itemRecreators.Add(new()
            { 
                ItemBaseID = item.name,
                Amount = item.count,
                Slot = AcceptedSlots.ItemsSlot
            });
        }
    }

    public static void AddToAmmosSlot(this List<ItemRecreator> itemRecreators, List<(string name, uint count)> values)
    {
        foreach (var item in values)
        {
            itemRecreators.Add(new()
            {
                ItemBaseID = item.name,
                Amount = item.count,
                Slot = AcceptedSlots.AmmoSlot
            });
        }
    }
}
