using EIV_JsonLib;

namespace EIV_Common.JsonStuff;

public static class RigHelper
{
    public static bool CheckCompatibleArmorPlate(this Rig rig, string armorPlateId)
    {
        // If this emtpy means we accept every plate!
        if (rig.ArmorPlateAccepted.Count == 0)
            return true;

        //  alternatively we check if it contains
        return rig.ArmorPlateAccepted.Contains(armorPlateId);
    }

    public static bool CheckCompatibleItemType(this Rig rig, string itemId)
    {
        // If this emtpy means we accept item!
        if (rig.ItemTypesAccepted.Count == 0)
            return true;

        //  alternatively we check if it contains
        return rig.ItemTypesAccepted.Contains(itemId);
    }

    public static bool CheckCompatibleItem(this Rig rig, string itemId)
    {
        // If this emtpy means we accept every plate!
        if (rig.SpecificItemsAccepted.Count == 0)
            return true;

        //  alternatively we check if it contains
        return rig.SpecificItemsAccepted.Contains(itemId);
    }


    public static bool TrySetArmorPlate(this Rig rig, string armorPlateId)
    {
        if (!rig.CheckCompatibleArmorPlate(armorPlateId))
            return false;

        rig.PlateSlotId = armorPlateId;
        return true;
    }

    public static bool TryAddItem(this Rig rig, string itemId)
    {
        if (!rig.CheckCompatibleItem(itemId))
            return false;

        var item = ItemMaker.MakeNewItem(itemId);
        if (item == null)
            return false;

        if (!rig.CheckCompatibleItemType(item.ItemType))
            return false;

        if (rig.ItemIds.Count == rig.MaxItem)
            return false;

        rig.ItemIds.Add(itemId);
        return true;
    }

    public static bool TryAddItems(this Rig rig, IList<string> itemId)
    {
        foreach (var item in itemId)
        {
            if (!rig.TryAddItem(item))
                return false;
        }
        return true;
    }
}
