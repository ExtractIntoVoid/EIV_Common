using EIV_JsonLib;
using EIV_JsonLib.Base;
using EIV_JsonLib.Extension;

namespace EIV_Common.JsonStuff;

public class ItemRemake
{
    public static List<ItemBase> ItemRemaker(List<ItemRecreator> itemsToRecreate)
    {
        List<ItemBase> ret = [];
        foreach (ItemRecreator item in itemsToRecreate)
        {
            ItemBase? remadeItem = ItemRemaker(item);
            if (remadeItem == null)
                continue;
            for (int i = 0; i < item.Amount; i++)
                ret.Add((ItemBase)remadeItem.Clone());
        }

        return ret;
    }

    public static ItemBase? ItemRemaker(ItemRecreator itemRecreator)
    {
        ItemBase? item = ItemMaker.MakeNewItem(itemRecreator.ItemBaseID);
        if (item == null)
            return null;

        foreach (KeyValuePair<string, KVChange> keyValuePair in itemRecreator.ChangedValues)
        {
            item.ChangeProperty(keyValuePair.Key, keyValuePair.Value);
        }


        foreach (ItemRecreator contaied in itemRecreator.Contained)
        {
            ItemBase? remadeItem = ItemRemaker(contaied);
            if (remadeItem == null)
                continue;

            switch (item.ItemType)
            {
                case nameof(Magazine):
                    {
                        Magazine mag = (Magazine)item;
                        if (contaied.Slot == AcceptedSlots.AmmoSlot)
                        {
                            mag.TryInsertAmmos(contaied.ItemBaseID, contaied.Amount);
                        }
                        item = mag;
                    }
                    break;
                case nameof(Gun):
                    {
                        Gun gun = (Gun)item;
                        if (contaied.Slot == AcceptedSlots.MagazineSlot)
                        {
                            gun.TryCreateMagazine(contaied.ItemBaseID);
                        }
                        item = gun;
                    }
                    break;
                case nameof(Rig):
                    {
                        Rig rig = (Rig)item;
                        if (contaied.Slot == AcceptedSlots.PlateSlot)
                        {
                            rig.TrySetArmorPlate(contaied.ItemBaseID);
                        }
                        if (contaied.Slot == AcceptedSlots.ItemsSlot)
                        {
                            rig.TryAddItem(contaied.ItemBaseID);
                        }
                        item = rig;
                    }
                    break;
                default:
                    break;
            }
        }

        return item;
    }
}
