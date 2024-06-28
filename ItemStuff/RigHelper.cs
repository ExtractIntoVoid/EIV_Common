using EIV_JsonLib.Interfaces;

namespace EIV_Common.ItemStuff
{
    public static class RigHelper
    {
        public static bool CheckCompatibleArmorPlate(this IRig rig, string ArmorPlateId)
        {
            if (rig == null)
                return false;

            // If this emtpy means we accept every plate!
            if (rig.ArmorPlateAccepted.Count == 0)
                return true;

            //  alternatively we check if it contains
            return rig.ArmorPlateAccepted.Contains(ArmorPlateId);
        }

        public static bool CheckCompatibleItemType(this IRig rig, string ItemId)
        {
            if (rig == null)
                return false;

            // If this emtpy means we accept item!
            if (rig.ItemTypesAccepted.Count == 0)
                return true;

            //  alternatively we check if it contains
            return rig.ItemTypesAccepted.Contains(ItemId);
        }

        public static bool CheckCompatibleItem(this IRig rig, string ItemId)
        {
            if (rig == null)
                return false;

            // If this emtpy means we accept every plate!
            if (rig.SpecificItemsAccepted.Count == 0)
                return true;

            //  alternatively we check if it contains
            return rig.SpecificItemsAccepted.Contains(ItemId);
        }


        public static bool TrySetArmorPlate(this IRig rig, string ArmorPlateId)
        {
            if (rig == null) return false;

            if (!rig.CheckCompatibleArmorPlate(ArmorPlateId))
                return false;

            rig.PlateSlotId = ArmorPlateId;
            return true;
        }

        public static bool TryAddItem(this IRig rig, string ItemId)
        {
            if (rig == null) return false;

            if (!rig.CheckCompatibleItem(ItemId))
                return false;

            var item = ItemMaker.MakeNewItem(ItemId);
            if (item == null)
                return false;

            if (!rig.CheckCompatibleItemType(item.ItemType))
                return false;

            if (rig.ItemIds.Count == rig.MaxItem)
                return false;

            rig.ItemIds.Add(ItemId);
            return true;
        }

        public static bool TryAddItems(this IRig rig, List<string> ItemId)
        {
            foreach (var item in ItemId)
            {
                if (!rig.TryAddItem(item))
                    return false;
            }
            return true;
        }
    }
}
