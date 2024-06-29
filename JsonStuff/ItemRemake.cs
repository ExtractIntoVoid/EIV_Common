using EIV_JsonLib.Classes;
using EIV_JsonLib.Interfaces;

namespace EIV_Common.ItemStuff
{
    public class ItemRemake
    {
        public static List<IItem> ItemRemaker(List<ItemRecreator> itemRecreator)
        {
            var ret = new List<IItem>();
            foreach (var item in itemRecreator)
            {
                var remadeItem = ItemRemaker(item);
                if (remadeItem == null)
                    continue;
                for (var i = 0; i < item.Amount; i++)
                    ret.Add((IItem)remadeItem.Clone());
            }

            return ret;
        }

        public static IItem? ItemRemaker(ItemRecreator itemRecreator)
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
                    case nameof(IMagazine):
                        {
                            var mag = (IMagazine)item;
                            if (contaied.Slot == AcceptedSlots.AmmoSlot)
                            {
                                mag.TryInsertAmmos(contaied.ItemBaseID, contaied.Amount);
                            }
                            item = mag;
                        }
                        break;
                    case nameof(IGun):
                        {
                            var gun = (IGun)item;
                            if (contaied.Slot == AcceptedSlots.MagazineSlot)
                            {
                                gun.TryCreateMagazine(contaied.ItemBaseID);
                            }
                            item = gun;
                        }
                        break;
                    case nameof(IRig):
                        {
                            var rig = (IRig)item;
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
}
