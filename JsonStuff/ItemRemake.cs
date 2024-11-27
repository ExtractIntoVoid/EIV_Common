using EIV_JsonLib;
using EIV_JsonLib.Base;

namespace EIV_Common.JsonStuff;

public class ItemRemake
{
    public static List<ItemBase> ItemRemaker(List<ItemRecreator> itemsToRecreate)
    {
        var ret = new List<ItemBase>();
        foreach (var item in itemsToRecreate)
        {
            var remadeItem = ItemRemaker(item);
            if (remadeItem == null)
                continue;
            for (var i = 0; i < item.Amount; i++)
                ret.Add((ItemBase)remadeItem.Clone());
        }

        return ret;
    }

    public static ItemBase? ItemRemaker(ItemRecreator itemRecreator)
    {
        var item = ItemMaker.MakeNewItem(itemRecreator.ItemBaseID);
        if (item == null)
            return null;

        foreach (var keyValuePair in itemRecreator.ChangedValues)
        {
            item.ChangeProperty(keyValuePair.Key, keyValuePair.Value);
        }


        foreach (var contaied in itemRecreator.Contained)
        {
            var remadeItem = ItemRemaker(contaied);
            if (remadeItem == null)
                continue;

            switch (item.ItemType)
            {
                case nameof(Magazine):
                    {
                        var mag = (Magazine)item;
                        if (contaied.Slot == AcceptedSlots.AmmoSlot)
                        {
                            mag.TryInsertAmmos(contaied.ItemBaseID, contaied.Amount);
                        }
                        item = mag;
                    }
                    break;
                case nameof(Gun):
                    {
                        var gun = (Gun)item;
                        if (contaied.Slot == AcceptedSlots.MagazineSlot)
                        {
                            gun.TryCreateMagazine(contaied.ItemBaseID);
                        }
                        item = gun;
                    }
                    break;
                case nameof(Rig):
                    {
                        var rig = (Rig)item;
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
