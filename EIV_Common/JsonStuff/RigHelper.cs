using EIV_JsonLib;
using EIV_JsonLib.Base;

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
        // If this emtpy means we accept any item!
        if (rig.ItemTypesAccepted.Count == 0)
            return true;

        //  alternatively we check if it contains
        return rig.ItemTypesAccepted.Contains(itemId);
    }

    public static bool CheckCompatibleItem(this Rig rig, string itemId)
    {
        // If this emtpy means we accept any item!
        if (rig.SpecificItemsAccepted.Count == 0)
            return true;

        //  alternatively we check if it contains
        return rig.SpecificItemsAccepted.Contains(itemId);
    }


    public static bool TrySetArmorPlate(this Rig rig, string armorPlateId)
    {
        if (!rig.CheckCompatibleArmorPlate(armorPlateId))
            return false;

        ArmorPlate? armorPlate = ItemMaker.CreateItem<ArmorPlate>(armorPlateId);
        if (armorPlate == null)
            return false;

        rig.PlateSlot = armorPlate;
        return true;
    }

    public static bool TryAddItem(this Rig rig, string itemId)
    {
        if (!rig.CheckCompatibleItem(itemId))
            return false;

        CoreItem? item = ItemMaker.MakeNewItem(itemId);
        if (item == null)
            return false;

        if (!rig.CheckCompatibleItemType(item.ItemType))
            return false;

        if (!rig.CanAddItem(item))
            return false;

        rig.Items.Add(item);
        return true;
    }

    public static bool TryAddItems(this Rig rig, IList<string> itemId)
    {
        foreach (string item in itemId)
        {
            if (!rig.TryAddItem(item))
                return false;
        }
        return true;
    }
}
