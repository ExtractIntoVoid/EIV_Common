using EIV_JsonLib;

namespace EIV_Common.JsonStuff;

public static class ItemRecreatorHelper
{
    public static void AddToSlot(this List<ItemRecreator> itemRecreators, List<(string name, uint count)> values, AcceptedSlots slot)
    {
        itemRecreators.AddRange(values.Select(item => new ItemRecreator { ItemBaseID = item.name, Amount = item.count, Slot = slot }));
    }
}
