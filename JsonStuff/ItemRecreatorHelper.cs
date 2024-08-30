using EIV_JsonLib.Classes;

namespace EIV_Common.JsonStuff;

public static class ItemRecreatorHelper
{
    public static void AddToSlot(this List<ItemRecreator> itemRecreators, List<(string name, uint count)> values, AcceptedSlots slot)
    {
        foreach (var item in values)
        {
            itemRecreators.Add(new()
            {
                ItemBaseID = item.name,
                Amount = item.count,
                Slot = slot
            });
        }
    }
}
